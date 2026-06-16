import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient, httpResource } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { IGoal, IMilestone, ICreateGoalDto, IUpdateGoalDto, ICreateMilestoneDto, IUpdateMilestoneDto } from '../models/goal.model';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { GOAL_CATEGORY_COLORS } from '../../../core/constants/goal.constants';

@Injectable({ providedIn: 'root' })
export class GoalService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiHost}/Goals`;

    goalsResource = httpResource<ApiResponse<IGoal[]>>(() => this.apiUrl);
    
    isLoading = computed(() => this.goalsResource.isLoading());

    goals = computed(() => {
        const rawGoals = this.goalsResource.value()?.data ?? [];
        return this.sortGoals(rawGoals.map(g => this.withSortedMilestones(g)));
    });

    // Computed
    // Status check is case-insensitive or exact. We will use 'Active' / 'InProgress' based on status.
    // Note: Backend default is "Pending" or active. Let's map it safely.
    activeGoals = computed(() => this.goals().filter(g => g.status !== 'Completed' && g.status !== 'Archived'));
    completedGoals = computed(() => this.goals().filter(g => g.status === 'Completed'));

    // ---- Sort helpers ----
    private sortGoals(goals: IGoal[]): IGoal[] {
        return [...goals].sort((a, b) =>
            new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
    }

    private sortMilestones(milestones: IMilestone[]): IMilestone[] {
        return [...milestones].sort((a, b) =>
            new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
    }

    private withSortedMilestones(goal: IGoal): IGoal {
        return { ...goal, milestones: this.sortMilestones(goal.milestones ?? []) };
    }

    getAllGoals(): Observable<IGoal[]> {
        return this.http.get<ApiResponse<IGoal[]>>(this.apiUrl).pipe(
            map(res => res.data ?? [])
        );
    }

    getGoalById(id: string): Observable<IGoal> {
        return this.http.get<ApiResponse<IGoal>>(`${this.apiUrl}/${id}`).pipe(
            map(res => res.data)
        );
    }

    getProgress(goal: IGoal): number {
        if (!goal.milestones?.length) return 0;
        const completed = goal.milestones.filter(m => m.isCompleted).length;
        return Math.round((completed / goal.milestones.length) * 100);
    }

    getCompletedMilestonesCount(goal: IGoal): number {
        return goal.milestones?.filter(m => m.isCompleted).length ?? 0;
    }

    getCategoryColor(category: string): string {
        return GOAL_CATEGORY_COLORS[category] ?? '#9ca3af';
    }

    getDaysLeft(goal: IGoal): number | null {
        if (!goal.targetDate) return null;
        const target = new Date(goal.targetDate);
        const today = new Date();
        const diff = Math.ceil((target.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
        return diff;
    }

    createGoal(dto: ICreateGoalDto): Observable<IGoal> {
        return this.http.post<ApiResponse<IGoal>>(this.apiUrl, dto).pipe(
            map(res => res.data),
            tap(newGoal => {
                this.goalsResource.update(res => {
                    if (!res) return { data: [newGoal], success: true, message: '' };
                    return { ...res, data: [newGoal, ...(res.data ?? [])] };
                });
            })
        );
    }

    updateGoal(id: string, dto: IUpdateGoalDto): Observable<IGoal> {
        return this.http.put<ApiResponse<IGoal>>(`${this.apiUrl}/${id}`, dto).pipe(
            map(res => res.data),
            tap(updatedGoal => {
                this.goalsResource.update(res => {
                    if (!res) return res;
                    return {
                        ...res,
                        data: (res.data ?? []).map(g => g.id === id ? updatedGoal : g)
                    };
                });
            })
        );
    }

    deleteGoal(id: string): Observable<void> {
        return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`).pipe(
            map(() => { }),
            tap(() => {
                this.goalsResource.update(res => {
                    if (!res) return res;
                    return {
                        ...res,
                        data: (res.data ?? []).filter(g => g.id !== id)
                    };
                });
            })
        );
    }

    addMilestone(goalId: string, dto: ICreateMilestoneDto): Observable<IMilestone> {
        return this.http.post<ApiResponse<IMilestone>>(`${this.apiUrl}/${goalId}/milestones`, dto).pipe(
            map(res => res.data),
            tap(newMilestone => {
                this.goalsResource.update(res => {
                    if (!res) return res;
                    return {
                        ...res,
                        data: (res.data ?? []).map(g => 
                            g.id === goalId 
                            ? { ...g, milestones: [...(g.milestones || []), newMilestone] }
                            : g
                        )
                    };
                });
            })
        );
    }

    updateMilestone(goalId: string, milestoneId: string, dto: IUpdateMilestoneDto): Observable<IMilestone> {
        return this.http.put<ApiResponse<IMilestone>>(`${this.apiUrl}/${goalId}/milestones/${milestoneId}`, dto).pipe(
            map(res => res.data),
            tap(updatedMilestone => {
                this.goalsResource.update(res => {
                    if (!res) return res;
                    return {
                        ...res,
                        data: (res.data ?? []).map(g => 
                            g.id === goalId 
                            ? { 
                                ...g, 
                                milestones: (g.milestones || []).map(m => m.id === milestoneId ? updatedMilestone : m) 
                              }
                            : g
                        )
                    };
                });
            })
        );
    }

    toggleMilestone(goalId: string, milestoneId: string): Observable<IMilestone> {
        const goal = this.goals().find(g => g.id === goalId);
        const milestone = goal?.milestones?.find(m => m.id === milestoneId);
        if (!milestone) throw new Error('Milestone not found');

        const updatedDto: IUpdateMilestoneDto = {
            title: milestone.title,
            description: milestone.description,
            targetDate: milestone.targetDate,
            isCompleted: !milestone.isCompleted
        };

        return this.updateMilestone(goalId, milestoneId, updatedDto);
    }

    deleteMilestone(goalId: string, milestoneId: string): Observable<void> {
        return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${goalId}/milestones/${milestoneId}`).pipe(
            map(() => { }),
            tap(() => {
                this.goalsResource.update(res => {
                    if (!res) return res;
                    return {
                        ...res,
                        data: (res.data ?? []).map(g => 
                            g.id === goalId 
                            ? { ...g, milestones: (g.milestones || []).filter(m => m.id !== milestoneId) }
                            : g
                        )
                    };
                });
            })
        );
    }
}

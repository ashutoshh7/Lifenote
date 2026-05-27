import { Injectable, signal, computed, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap, map } from 'rxjs';
import { IGoal, IMilestone, ICreateGoalDto, IUpdateGoalDto, ICreateMilestoneDto, IUpdateMilestoneDto } from '../models/goal.model';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { GOAL_CATEGORY_COLORS } from '../../../core/constants/goal.constants';

@Injectable({ providedIn: 'root' })
export class GoalService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiHost}/Goals`;

    // Signals
    goals = signal<IGoal[]>([]);
    isLoading = signal(false);

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
        this.isLoading.set(true);
        return this.http.get<ApiResponse<IGoal[]>>(this.apiUrl).pipe(
            map(res => res.data ?? []),
            tap(goals => {
                this.goals.set(this.sortGoals(goals.map(g => this.withSortedMilestones(g))));
                this.isLoading.set(false);
            })
        );
    }

    getGoalById(id: number): Observable<IGoal> {
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
                // newest goal goes to the top
                this.goals.update(gs => [this.withSortedMilestones(newGoal), ...gs]);
            })
        );
    }

    updateGoal(id: number, dto: IUpdateGoalDto): Observable<IGoal> {
        return this.http.put<ApiResponse<IGoal>>(`${this.apiUrl}/${id}`, dto).pipe(
            map(res => res.data),
            tap(updatedGoal => {
                this.goals.update(gs => gs.map(g => g.id === id ? this.withSortedMilestones(updatedGoal) : g));
            })
        );
    }

    deleteGoal(id: number): Observable<void> {
        return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`).pipe(
            map(() => { }),
            tap(() => {
                this.goals.update(gs => gs.filter(g => g.id !== id));
            })
        );
    }

    addMilestone(goalId: number, dto: ICreateMilestoneDto): Observable<IMilestone> {
        return this.http.post<ApiResponse<IMilestone>>(`${this.apiUrl}/${goalId}/milestones`, dto).pipe(
            map(res => res.data),
            tap(newMilestone => {
                this.goals.update(gs => gs.map(g =>
                    g.id === goalId
                        ? { ...g, milestones: this.sortMilestones([...(g.milestones || []), newMilestone]) }
                        : g
                ));
            })
        );
    }

    updateMilestone(goalId: number, milestoneId: number, dto: IUpdateMilestoneDto): Observable<IMilestone> {
        return this.http.put<ApiResponse<IMilestone>>(`${this.apiUrl}/${goalId}/milestones/${milestoneId}`, dto).pipe(
            map(res => res.data),
            tap(updatedMilestone => {
                this.goals.update(gs => gs.map(g =>
                    g.id === goalId
                        ? {
                            ...g,
                            milestones: g.milestones.map(m => m.id === milestoneId ? updatedMilestone : m)
                        }
                        : g
                ));
            })
        );
    }

    toggleMilestone(goalId: number, milestoneId: number): Observable<IMilestone> {
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

    deleteMilestone(goalId: number, milestoneId: number): Observable<void> {
        return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${goalId}/milestones/${milestoneId}`).pipe(
            map(() => { }),
            tap(() => {
                this.goals.update(gs => gs.map(g =>
                    g.id === goalId
                        ? { ...g, milestones: g.milestones.filter(m => m.id !== milestoneId) }
                        : g
                ));
            })
        );
    }
}

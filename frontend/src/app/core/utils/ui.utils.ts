export class UIUtils {
  static getNoteIcon(content?: string): string {
    const text = content?.toLowerCase() ?? '';
    if (text.includes('meeting') || text.includes('sync')) return 'meeting_room';
    if (text.includes('idea') || text.includes('brainstorm')) return 'lightbulb';
    if (text.includes('todo') || text.includes('task')) return 'format_list_bulleted';
    return 'article';
  }
}

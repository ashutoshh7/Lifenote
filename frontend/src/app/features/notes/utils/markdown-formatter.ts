/**
 * Applies markdown syntax wrapping or prefix to the selected text in a textarea.
 * Returns the new content, and the new cursor selection range.
 */
export function formatMarkdownText(
  content: string,
  start: number,
  end: number,
  type: 'bold' | 'italic' | 'ul' | 'ol'
): { newContent: string; newCursorStart: number; newCursorEnd: number } {
  const selected = content.substring(start, end);
  let newContent = content;
  let newCursorStart = start;
  let newCursorEnd = end;

  if (type === 'bold') {
    const wrapped = `**${selected || 'bold text'}**`;
    newContent = content.substring(0, start) + wrapped + content.substring(end);
    newCursorStart = start + 2;
    newCursorEnd = start + 2 + (selected || 'bold text').length;
  } else if (type === 'italic') {
    const wrapped = `*${selected || 'italic text'}*`;
    newContent = content.substring(0, start) + wrapped + content.substring(end);
    newCursorStart = start + 1;
    newCursorEnd = start + 1 + (selected || 'italic text').length;
  } else if (type === 'ul' || type === 'ol') {
    // Split selected lines and prefix each
    const lines = selected
      ? selected.split('\n').map((line, i) =>
          type === 'ul' ? `- ${line}` : `${i + 1}. ${line}`
        ).join('\n')
      : type === 'ul' ? '- List item' : '1. List item';

    // Ensure there's a newline before the list if not at start of line
    const before = content.substring(0, start);
    const prefix = before.length > 0 && !before.endsWith('\n') ? '\n' : '';
    newContent = before + prefix + lines + '\n' + content.substring(end);
    newCursorStart = start + prefix.length;
    newCursorEnd = newCursorStart + lines.length;
  }

  return { newContent, newCursorStart, newCursorEnd };
}

export interface INote {
  id: string;
  userid: string;
  title: string;
  content: string;
  category?: string;
  tags?: string[];
  isPinned?: boolean;
  isArchived?: boolean;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface ICreateNoteDto {
  title: string;
  content: string;
  category?: string;
  tags?: string[];
}

export interface IUpdateNoteDto {
  title?: string;
  content?: string;
  category?: string;
  tags?: string[];
  isPinned?: boolean;
  isArchived?: boolean;
}

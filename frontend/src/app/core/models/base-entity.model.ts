// Base interface - this is a common pattern for entities
export interface BaseEntity {
  id: string;
  createdAt: Date;
  updatedAt: Date;
}

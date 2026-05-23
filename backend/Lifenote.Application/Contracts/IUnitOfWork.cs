// IUnitOfWork has been removed from Application.Contracts.
// The canonical definition and single source of truth is:
//   Lifenote.Domain.Interfaces.IUnitOfWork
// All Application services must inject Lifenote.Domain.Interfaces.IUnitOfWork directly.
// This file is retained only to preserve git history — it can be deleted in Phase 3 cleanup.
namespace Lifenote.Application.Contracts;

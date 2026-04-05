using Lifenote.Domain.Interfaces;

namespace Lifenote.Application.Contracts;

/// <summary>
/// Re-exported for Application layer convenience.
/// The canonical definition lives in Lifenote.Domain.Interfaces.IUnitOfWork.
/// Application services should inject Lifenote.Domain.Interfaces.IUnitOfWork directly.
/// This file is kept only for backward-compatibility during the refactor — remove once
/// all Application services import from Lifenote.Domain.Interfaces.
/// </summary>
[Obsolete("Inject Lifenote.Domain.Interfaces.IUnitOfWork directly. This re-export will be removed.")]
public interface IUnitOfWork : Lifenote.Domain.Interfaces.IUnitOfWork { }

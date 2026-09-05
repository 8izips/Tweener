# Tweener

Tweener is a lightweight Unity tween / sequence system.

This repository now uses the new module-based architecture.

## Structure

- `Tweener.cs` — runtime playback and timeline control
- `SequenceModule.cs` — base contract for sequence modules
- `Tweener.EaseFunctions.cs` — easing LUTs and easing functions
- `Tweener.Modules.cs` — module discovery / factory
- `DefaultModules/` — built-in modules
- `CustomModules/` — project-specific modules
- `Editor/` — custom inspector and preview

## Built-in modules

- CanvasGroup Alpha
- GameObject Enable
- RectTransform Move / Rotate / Scale
- Transform Move / Rotate / Scale

## License

See `LICENSE`.

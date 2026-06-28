; Unshipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
RD0001 | RhythmBase.Generator | Error | InvalidConverterRegistrationRule
RD0002 | RhythmBase.Generator | Error | DuplicateConverterRegistrationRule
RD0003 | RhythmBase.Generator | Error | InvalidEnumTypeRule
RD0004 | RhythmBase.Generator | Error | MissingEnumInitializerRule
RD0005 | RhythmBase.Generator | Error | MissingFallbackModelRule
RD0006 | RhythmBase.Generator | Error | FallbackModelMissingParameterlessCtorRule
RD0007 | RhythmBase.Generator | Error | MultipleFallbackModelsRule
RD0008 | RhythmBase.Generator | Error | InvalidNamespaceIdRule
RD0009 | RhythmBase.Generator | Error | MissingTypeRegistrationRule
RD0010 | RhythmBase.Generator | Error | NonGenericBaseConverterRule
RD0011 | RhythmBase.Generator | Error | GenericTypeMissingConverterRule
RD0012 | RhythmBase.Generator | Error | GenericConverterBaseMustBeNonGenericRule

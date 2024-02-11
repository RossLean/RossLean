# Diagnostic Groups
Diagnostic groups contain a collection of diagnostics, grouped together due to their common characteristics. The grouped diagnostics will appear in the same place from the same feature, where their difference may be specific details about the diagnostic.

Diagnostic groups can either be distinctive or declarative.

## Distinctive
Only up to one of the grouped diagnostics may be emitted at a time per examined element. This means that a piece of code that could produce more than one of those diagnostics, will only have the most fitting one emitted. Multiple code elements in the same file may contain different instances of these diagnostics.

## Declarative
There is no restriction on what diagnostics will be emitted.

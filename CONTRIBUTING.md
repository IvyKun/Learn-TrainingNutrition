# Contributing

## Code Standards

### Naming — C#
- Classes, methods, properties: `PascalCase`
- Private fields: `_camelCase`
- Local variables, parameters: `camelCase`
- Interfaces: `IMyInterface`
- Async methods: suffix with `Async` — `GetUserAsync()`

### C# Best Practices
- Always use `async/await` for I/O — never `.Result` or `.Wait()`
- Always pass and respect `CancellationToken` in async methods
- Use `record` types for Value Objects and DTOs
- Prefer `IReadOnlyList<T>` over `List<T>` for return types
- Do not use `var` when the type is not obvious from the right side
- Nullable reference types are enabled — handle nulls explicitly

### Architecture Rules
- No business logic in Endpoints — ever
- No EF Core references in the Application layer — ever
- No direct `new` for dependencies — always inject via constructor
- One class per file
- Keep methods short — if a method needs a comment to explain what it does, extract it

### Testing Rules
- Every handler must have unit tests
- Tests follow **Arrange / Act / Assert** with section comments
- Test method names follow: `MethodName_StateUnderTest_ExpectedBehavior`
- Test behavior, not implementation details

### Frontend — TypeScript / TSX
- Double quotes throughout — matches Prettier default and shadcn generated code
- Backtick only when interpolating with `${}`
- All identifiers and comments in English

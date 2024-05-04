## Introduction

Welcome to the **SpelCompiler**! This library aims to provide .NET developers with the ability to compile and execute Spring Expression Language (SpEL) expressions,for now, particularly focusing on 'where' expressions.

## Features

- Compile 'where' expressions from SpEL to .NET
- Execute compiled expressions in a .NET environment

- Usage
Here’s a basic example of how to use the library:

C#

using SpelCompiler;

// Create an instance of the compiler
var compiler = new SpelGrammerCompiler<TestModel>();

// Compile a 'where' expression
var compiledExpression = compiler.CreateFunc("age > 45");

// Execute the compiled expression
var result = _models.Where(compiledExpression.Compile()).ToArray();

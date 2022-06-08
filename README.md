
# What is it
Ferb.Template is a basic asp-like templating engine geared towards usage in a scripting environment like Linqpad or dotnet-script 
# TLDR;
## Syntax
This library implements a templating engine similar to classic ASP using `<%` and `%>` to deliniate code blocks, `<%=` to implicitly write the result of the expression to the output, `<%#` to define a using statement, `<%&` to invoke a named template, and `<%@` to define template members.
## Invocation
Import the default template engine setup with: `using static Ferb.Template.Engine;` 
Invoke a template with `var output = TemplateEngine().Exec("widgets.html",widgetWarehouse);`

# Syntax
The templating system implements an syntax similar to classic ASP / PHP. C# code can be enclosed between `<%` and `%>`. Every template has two read-only, template-scope properties, `Output` a TextWriter and `Context` a TContext which is specified at template instantiation.
a simple list of numbers can be printed as such:
```
<ul>
<%foreach(var widget in Context.AvailableWidgets) {%>
	<li><% Output.Write(widget.Name); %></li>
<%}%>
</ul>
```

There are also a few types of code blocks with special behavior which can be defined by appending a character directly after the opening deliniator. 

A `<%=` codeblock is evaluated as an expression to be passed into `Output.Write()`
A `<%#` codeblock is evaluated as C# using statement
A `<%@` codeblock is converted into a class member such as field, property or function.
a `<%&` invokes a sub-template. This code block is expected to have a the file name of the template, followed by a `;`, followed by an expression to be evaluated as the template context
```
<ul>
<%foreach(var widget in Context.AvailableWidgets) {%>
	<%&AvailableWidget; widget %>
<%}%>
</ul>
```

# Under the Hood
Internally the template engine parses the template into a series of string literals and code blocks. Code blocks are then arranged into a dynamically compiled class inheriting from `Ferb.Template.TemplateBase<TContext>`. This class defines 4 protected virtual functions which you can override to customize funtionality:
    `protected virtual void ClassInit() { }` Allows for class initialization -- called during the class constructor.
    `protected virtual void BeforeExec() { }` Called after context and output are set-up but before the template is executed
    `protected virtual void AfterExec() { }` Called after the template is executed but before the output is converted to a string.
    `protected virtual string Transform(string outputText)` Allows for transforming the output as a single string before emitting it.
This class also has a member `protected static readonly string[] __literal_chunks` which is initialized with all the literal values to be output during template evaluation


# Using the Source Generator 
The library also includes a source generator to support pre-compiled templates. Each precompiled template will need to be a `partial class` inherit from `PrecompiledTemplateBase<TContext>` and be 
decorated with an attribute `[TemplateSource("<path>")]` where path is a file relative to the root of the project containing the template contents. 


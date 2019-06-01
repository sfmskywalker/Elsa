﻿using Elsa.Attributes;
using Elsa.Expressions;
using Elsa.Models;

namespace Elsa.Activities.Console.Activities 
{
    /// <summary>
    /// Writes a text string to the console.
    /// </summary>
    [DisplayName("Write Line")]
    [Category("Console")]
    [Description("Write a line to the console")]
    public class WriteLine : Activity
    {
        public WriteLine()
        {
        }

        public WriteLine(string text) : this(new WorkflowExpression<string>(PlainTextEvaluator.SyntaxName, text))
        {
        }
        
        public WriteLine(WorkflowExpression<string> textExpression)
        {
            TextExpression = textExpression;
        }
        
        public WorkflowExpression<string> TextExpression { get; set; }
    }
}

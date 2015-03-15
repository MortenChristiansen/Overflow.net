using System;
using System.Collections.Generic;
using System.IO;
using Overflow.Extensibility;

namespace Overflow
{
    public class TextWriterWorkflowLogger : IWorkflowLogger
    {
        private readonly TextWriter _writer;

        public TextWriterWorkflowLogger(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException("writer");

            _writer = writer;
        }

        private readonly Stack<LevelInfo> _levelInfo = new Stack<LevelInfo>(); 

        public void OperationStarted(IOperation operation)
        {
            PrepareForChildItem();

            _writer.Write(operation.GetType().Name);

            var levelInfo = new LevelInfo();
            _levelInfo.Push(levelInfo);
        }

        private void PrepareForChildItem()
        {
            if (_levelInfo.Count > 0)
            {
                if (_levelInfo.Peek().Children == 0)
                    _writer.Write(" {");

                _writer.WriteLine();

                if (_levelInfo.Peek().Children > 0)
                    _writer.WriteLine();

                _levelInfo.Peek().Children++;
            }
            AddNesting();
        }

        private void AddNesting()
        {
            _writer.Write(new string(' ', 2 * _levelInfo.Count));
        }

        public void OperationFinished(IOperation operation)
        {
            if (_levelInfo.Count == 0)
                throw new InvalidOperationException("No operation was logged as started so a finished operation cannot be logged.");

            var levelInfo = _levelInfo.Pop();

            if (levelInfo.Children > 0)
                _writer.Write(Environment.NewLine + "}");
        }

        public void OperationFailed(IOperation operation, Exception error)
        {
            if (_levelInfo.Count == 0)
                throw new InvalidOperationException("No operation was logged as started so an operation failure cannot be logged.");

            PrepareForChildItem();

            _writer.Write("Error [" + error.GetType().Name + "]: " + error.Message);
        }

        public void BehaviorWasApplied(IOperation operation, OperationBehavior behavior, string description)
        {
            if (_levelInfo.Count == 0)
                throw new InvalidOperationException("No operation was logged as started so an operation behavior cannot be logged.");
            
            PrepareForChildItem();

            _writer.Write(behavior.GetType().Name + ": " + description);
        }

        private class LevelInfo
        {
            public int Children { get; set; }
        }
    }
}

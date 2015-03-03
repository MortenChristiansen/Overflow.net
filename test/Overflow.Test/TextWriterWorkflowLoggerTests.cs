using System;
using System.IO;
using Overflow.Test.Fakes;
using Xunit;

namespace Overflow.Test
{
    public class TextWriterWorkflowLoggerTests
    {
        private static readonly string NL = Environment.NewLine;

        [Fact]
        public void Logging_an_operation_start_writes_the_operation_type_to_the_output()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());

                Assert.Equal("FakeOperation", sw.ToString());
            }
        }

        [Fact]
        public void You_cannot_create_a_logger_without_a_text_writer()
        {
            Assert.Throws<ArgumentNullException>(() => new TextWriterWorkflowLogger(null));
        }

        [Fact]
        public void Logging_an_operation_start_and_finish_writes_the_operation_type_to_the_output()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());

                Assert.Equal("FakeOperation", sw.ToString());
            }
        }

        [Fact]
        public void Logging_an_operation_start_before_the_previous_operation_was_finished_writes_a_beggining_brace_and_the_new_operation_type_indented_to_the_output()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());

                Assert.Equal(string.Format("FakeOperation {{{0}  FakeOperation", NL), sw.ToString());
            }
        }

        [Fact]
        public void Logging_an_operation_starting_and_finishing_within_another_operation_start_and_finish_writes_the_inner_operation_type_name_nested_in_braces_to_the_output()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation());
                sut.OperationFinished(new FakeOperation());

                Assert.Equal(string.Format("FakeOperation {{{0}  FakeOperation{0}}}", NL), sw.ToString());
            }
        }

        [Fact]
        public void Logging_multiple_operation_on_the_same_level_adds_a_seperation_line_between_them()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation());
                sut.OperationFinished(new FakeOperation());

                Assert.Equal(string.Format("FakeOperation {{{0}  FakeOperation{0}{0}  FakeOperation{0}}}", NL), sw.ToString());
            }
        }

        [Fact]
        public void Nesting_increased_by_two_speces_for_every_started_unfinished_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());

                Assert.Equal(string.Format("FakeOperation {{{0}  FakeOperation {{{0}    FakeOperation {{{0}      FakeOperation", NL), sw.ToString());
            }
        }

        [Fact]
        public void Failures_are_logged_nested_between_braces()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationFailed(new FakeOperation(), new InvalidOperationException("MESSAGE"));
                sut.OperationFinished(new FakeOperation());

                Assert.Equal(string.Format("FakeOperation {{{0}  Error [InvalidOperationException]: MESSAGE{0}}}", NL), sw.ToString());
            }
        }

        [Fact]
        public void Failures_are_correctly_indented_after_a_sibling_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation());
                sut.OperationFailed(new FakeOperation(), new InvalidOperationException("MESSAGE"));
                sut.OperationFinished(new FakeOperation());

                Assert.Equal(string.Format("FakeOperation {{{0}  FakeOperation{0}{0}  Error [InvalidOperationException]: MESSAGE{0}}}", NL), sw.ToString());
            }
        }

        [Fact]
        public void You_cannot_log_a_failure_without_a_started_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                Assert.Throws<InvalidOperationException>(() => sut.OperationFailed(new FakeOperation(), new Exception()));
            }
        }

        [Fact]
        public void You_cannot_log_a_failure_when_the_last_operation_has_been_logged_as_finished()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation());

                Assert.Throws<InvalidOperationException>(() => sut.OperationFailed(new FakeOperation(), new Exception()));
            }
        }

        [Fact]
        public void You_cannot_log_a_finished_operation_without_a_started_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                Assert.Throws<InvalidOperationException>(() => sut.OperationFinished(new FakeOperation()));
            }
        }

        [Fact]
        public void You_cannot_log_a_finished_operation_when_the_last_operation_has_already_been_logged_as_finished()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation());

                Assert.Throws<InvalidOperationException>(() => sut.OperationFinished(new FakeOperation()));
            }
        }
    }
}

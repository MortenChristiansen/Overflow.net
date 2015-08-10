using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Overflow.Test.Fakes;
using Overflow.Test.TestingInfrastructure;
using Ploeh.AutoFixture;
using Xunit;

namespace Overflow.Test
{
    public class TextWriterWorkflowLoggerTests : TestBase
    {
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
        public void Guards_are_verified()
        {
            Fixture.Register(() => Console.Out);

            VerifyGuards<TextWriterWorkflowLogger>();
        }

        [Fact]
        public void Logging_an_operation_start_and_finish_writes_the_operation_type_to_the_output()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal("FakeOperation [duration: 0ms]", sw.ToString());
            }
        }

        [Fact]
        public void The_execution_duration_is_added_in_milliseconds()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.FromMilliseconds(15));

                Assert.Equal("FakeOperation [duration: 15ms]", sw.ToString());
            }
        }

        [Fact]
        public void Log_durations_are_properly_formatted_according_to_cultures_using_period_as_thousands_seperator()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);
                Thread.CurrentThread.CurrentCulture = new CultureInfo("da-DK");

                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.FromMilliseconds(1500000));

                Assert.Equal("FakeOperation [duration: 1.500.000ms]", sw.ToString());
            }
        }

        [Fact]
        public void Log_durations_are_properly_formatted_according_to_cultures_using_comma_as_thousands_seperator()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.FromMilliseconds(1500000));

                Assert.Equal("FakeOperation [duration: 1,500,000ms]", sw.ToString());
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

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation", sw.ToString());
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
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation [duration: 0ms]{NL}}} [duration: 0ms]", sw.ToString());
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
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation [duration: 0ms]{NL}{NL}  FakeOperation [duration: 0ms]{NL}}} [duration: 0ms]", sw.ToString());
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

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation {{{NL}    FakeOperation {{{NL}      FakeOperation", sw.ToString());
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
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  Error [InvalidOperationException]: MESSAGE{NL}}} [duration: 0ms]", sw.ToString());
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
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.OperationFailed(new FakeOperation(), new InvalidOperationException("MESSAGE"));
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation [duration: 0ms]{NL}{NL}  Error [InvalidOperationException]: MESSAGE{NL}}} [duration: 0ms]", sw.ToString());
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
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Throws<InvalidOperationException>(() => sut.OperationFailed(new FakeOperation(), new Exception()));
            }
        }

        [Fact]
        public void You_cannot_log_a_finished_operation_without_a_started_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                Assert.Throws<InvalidOperationException>(() => sut.OperationFinished(new FakeOperation(), TimeSpan.Zero));
            }
        }

        [Fact]
        public void You_cannot_log_a_finished_operation_when_the_last_operation_has_already_been_logged_as_finished()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Throws<InvalidOperationException>(() => sut.OperationFinished(new FakeOperation(), TimeSpan.Zero));
            }
        }

        [Fact]
        public void Behaviors_are_logged_nested_between_braces()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.BehaviorWasApplied(new FakeOperation(), new FakeOperationBehavior(), "DESCRIPTION");
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  FakeOperationBehavior: DESCRIPTION{NL}}} [duration: 0ms]", sw.ToString());
            }
        }

        [Fact]
        public void Behaviors_are_correctly_indented_after_a_sibling_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.BehaviorWasApplied(new FakeOperation(), new FakeOperationBehavior(), "DESCRIPTION");
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation [duration: 0ms]{NL}{NL}  FakeOperationBehavior: DESCRIPTION{NL}}} [duration: 0ms]", sw.ToString());
            }
        }

        [Fact]
        public void You_cannot_log_a_behavior_without_a_started_operation()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                Assert.Throws<InvalidOperationException>(() => sut.BehaviorWasApplied(new FakeOperation(), new FakeOperationBehavior(), "DESCRIPTION"));
            }
        }

        [Fact]
        public void You_cannot_log_a_behavior_when_the_last_operation_has_been_logged_as_finished()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Throws<InvalidOperationException>(() => sut.BehaviorWasApplied(new FakeOperation(), new FakeOperationBehavior(), "DESCRIPTION"));
            }
        }

        [Fact]
        public void Nested_operations_are_correctly_indented()
        {
            using (var sw = new StringWriter())
            {
                var sut = new TextWriterWorkflowLogger(sw);

                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationStarted(new FakeOperation());
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);
                sut.OperationFinished(new FakeOperation(), TimeSpan.Zero);

                Assert.Equal($"FakeOperation {{{NL}  FakeOperation {{{NL}    FakeOperation [duration: 0ms]{NL}  }} [duration: 0ms]{NL}}} [duration: 0ms]", sw.ToString());
            }
        }
    }
}

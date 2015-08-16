using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace Overflow.Test.TestingInfrastructure
{
    public abstract class TestBase
    {
        protected static readonly string NL = Environment.NewLine;

        public IFixture Fixture { get; }

        protected TestBase()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        protected void VerifyConstructorGuards()
        {
            var assertion = new GuardClauseAssertion(Fixture);
            assertion.Verify(GetType().GetConstructors());
        }

        protected void VerifyMethodGuards<TSut>()
        {
            var assertion = new GuardClauseAssertion(Fixture);
            assertion.Verify(typeof(TSut).GetMethods());
        }

        protected void VerifyGuards<TSut>()
        {
            VerifyConstructorGuards();
            VerifyMethodGuards<TSut>();
        }

        protected void ExecuteIgnoringErrors(Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
            }
        }
    }
}

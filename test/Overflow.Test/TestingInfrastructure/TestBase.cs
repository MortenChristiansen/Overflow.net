using System;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Idioms;

namespace Overflow.Test.TestingInfrastructure
{
    public abstract class TestBase
    {
        protected static readonly string NL = Environment.NewLine;

        public IFixture Fixture { get; private set; }

        protected TestBase()
        {
            Fixture = new Fixture().Customize(new AutoMoqCustomization());
        }

        protected void VerifyConstructorGuards<TSut>()
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
            VerifyConstructorGuards<TSut>();
            VerifyMethodGuards<TSut>();
        }
    }
}

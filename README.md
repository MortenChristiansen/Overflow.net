#Overflow.net

[![Build Status](https://travis-ci.org/MortenChristiansen/Overflow.net.svg?branch=creating-operations-and-resolving-dependencies)](https://travis-ci.org/MortenChristiansen/Overflow.net)

Overflow is an open source C# portable class library for modeling workflows using simple objects. It allows you to define workflows using a simple and clean syntax, while you apply operational behaviors such as retry mode in a declarative fashion using attributes.

    public class SendPartyInvitesOperation : Operation
    {
        public override IEnumerable<IOperation> GetChildOperations()
        {
            yield return Create<FindGuestListOperation>();
            yield return Create<PrepareInviteTemplateOperation>();
            yield return Create<CreateInviteNotificationsOperation>();
            yield return Create<SendNotificationsOperation>();
        }
    }

    [RetryOnFailure(times = 3)]
    public class FindGuestListOperation : Operation
    {
        protected override void OnExecute()
        {
            ...
        }
    }

    public static class PartyWorkflows
    {
        public static void SendPartyInvites()
        {
            var workflow = new SendPartyInvitesOperation();
            workflow.Execute();
        }
    }

The library is in the very early stages of development and is still very much in a state of flux while the central APIs get defined.

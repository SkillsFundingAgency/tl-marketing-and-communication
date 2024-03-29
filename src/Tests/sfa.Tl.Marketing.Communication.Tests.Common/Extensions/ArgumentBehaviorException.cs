﻿using AutoFixture.Idioms;

namespace sfa.Tl.Marketing.Communication.Tests.Common.Extensions;

public class ArgumentBehaviorException : IBehaviorExpectation
{
    public void Verify(IGuardClauseCommand command)
    {
        try
        {
            command.Execute(null);
        }
        catch (ArgumentNullException)
        {
            return;
        }
        catch (ArgumentException)
        {
            return;
        }

        throw new GuardClauseException();
    }
}
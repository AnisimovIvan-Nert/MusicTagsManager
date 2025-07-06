using System;
using System.Threading.Tasks;
using CompileTimeWeaver;

namespace Desktop.CallTraceLogger;

public class ConsoleCallTraceLogger : AdviceAttribute
{
    private const int IndentSize = 4;
    private const char IndentChar = '-';

    private static int _currentNesting;

    private static Action<string>? WriteLine =>
#if DEBUG
        ConsoleWriteLine;
#else
        null;
#endif

    public override object Advise(IInvocation invocation)
    {
        var callingName = invocation.Instance + "." + invocation.Method.Name;

        WriteLine?.Invoke("Entering->" + callingName);
        AppendNesting();
        try
        {
            return invocation.Proceed();
        }
        catch (Exception)
        {
            WriteLine?.Invoke("Exception thrown");
            throw;
        }
        finally
        {
            ReduceNesting();
            WriteLine?.Invoke("Leaving " + callingName);
        }
    }

    public override async Task<object> AdviseAsync(IInvocation invocation)
    {
        var callingName = invocation.Instance + "." + invocation.Method.Name;

        WriteLine?.Invoke("Entering->" + callingName);
        AppendNesting();
        try
        {
            return await invocation.ProceedAsync();
        }
        catch (Exception)
        {
            WriteLine?.Invoke("Exception thrown");
            throw;
        }
        finally
        {
            ReduceNesting();
            WriteLine?.Invoke("Leaving " + callingName);
        }
    }

    private static void ConsoleWriteLine(string text)
    {
        var indent = new string(IndentChar, _currentNesting * IndentSize);
        Console.WriteLine(indent + text);
    }

    private static void AppendNesting()
    {
        _currentNesting++;
    }

    private static void ReduceNesting()
    {
        _currentNesting--;
    }
}
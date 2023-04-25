using ReactiveUI;

namespace Basyc.ReactiveUi;
public static class BasycReactiveUi
{
    /// <summary>
    ///     Magic code that is required to be references (sed the code does not even need to be called) in entry assembly in order to make ReactiveUI work.
    ///     If removed raising property change events of [Reactive] properties does not work. Finding backing field of [Reactive] properties will throw.
    ///     You can call this anywhere but expected in Program.cs or App.xaml.cs
    ///     (It adds ReactiveUI assembly reference directly to the calling assembly and fixing ReactiveUI.Fody weaving).
    /// </summary>
    public static ReactiveObject? Fix() => null;
}

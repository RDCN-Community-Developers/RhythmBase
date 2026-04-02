namespace RhythmBase.BeatBlock.Events
{
    public record class SetBoolean : BaseEvent
    {
        public override Enums Type => Enums.SetBoolean;
        public string Var { get; set; }
        public bool Enable { get; set; }
    }
}

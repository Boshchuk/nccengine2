namespace NccEngine2.GameComponents.Graphics.Screens.Menu
{
    public class BoolMenuEntry : MenuEntry
    {
        // Properties.
        public bool Value { get; set; }
        public string Label { get; set; }


        /// <summary>
        /// Constructor.
        /// </summary>
        public BoolMenuEntry(string label) 
        {
            Label = label;
        }


        /// <summary>
        /// Click handler toggles the boolean value.
        /// </summary>
        public override void OnClicked()
        {
            Value = !Value;

            base.OnClicked();
        }


        /// <summary>
        /// Customize our text string.
        /// </summary>
        public override string Text
        {
            get { return Label + " " + (Value ? "on" : "off"); }
            set { }
        }
    }
}
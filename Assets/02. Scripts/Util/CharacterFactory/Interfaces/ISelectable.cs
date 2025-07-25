using HighlightPlus;

//[RequireComponent(typeof(HighlightEffect))]
public interface ISelectable
{
    public HighlightEffect HighlightEffect { get; set; }
    public void SetHighlight(bool isOn);
}
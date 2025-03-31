using System;

[Serializable]
public class PortData {
    public string Guid;
    public string Text;
    public PortType Type;

    public PortData(string text, PortType type)
    {
        Guid = System.Guid.NewGuid().ToString();
        Text = text;
        Type = type;
    }

    public PortData(PortType type)
    {
        Guid = System.Guid.NewGuid().ToString();
        Type = type;
    }
}
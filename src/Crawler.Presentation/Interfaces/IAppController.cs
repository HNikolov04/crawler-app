namespace Crawler.Presentation.Interfaces;

public interface IAppController
{
    public string ToLower(string input);
    public string SplitCommandType(string input);
    public void LoadFile(string input);
    public void NewFile(string input);
    public void PrintNodes(string input);
    public void SetNodes(string input);
    public void CopyNodes(string input);
    public void Save(string input);
    public void Visualize();
    public bool ChangeFile();
    public void Clear();
    public void Exit();
}
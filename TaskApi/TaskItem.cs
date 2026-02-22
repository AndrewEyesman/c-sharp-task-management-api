public class TaskItem
{
    public int Id { get; set; }
    
    // C# 14: Use the 'field' keyword for easy validation logic
    public string Title { 
        get; 
        set => field = string.IsNullOrWhiteSpace(value) ? "Untitled Task" : value; 
    }
    
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
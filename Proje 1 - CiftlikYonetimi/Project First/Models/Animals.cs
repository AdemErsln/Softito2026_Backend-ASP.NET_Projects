namespace Project_First.Models;

public class Animals
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int AnimalTypeId { get; set; }


    public AnimalTypes AnimalType { get; set; } = null!;
}
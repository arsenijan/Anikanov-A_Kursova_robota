namespace Main.DTO
{
    public class DisciplineModel
    {
        public int DisciplineId { get; set; }
        public string Name { get; set; }
        public string Tasks { get; set; }
        public string Department { get; set; }
        public IFormFile Image { get; set; }
    }
}

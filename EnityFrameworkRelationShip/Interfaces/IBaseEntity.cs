namespace EnityFrameworkRelationShip.Interfaces
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}

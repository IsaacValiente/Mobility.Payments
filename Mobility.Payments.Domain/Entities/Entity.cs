namespace Mobility.Payments.Domain.Entities
{
    using System;

    public abstract class Entity
    {
        protected Entity()
        {
            this.SysCreatedOn = DateTime.UtcNow;
        }
        public virtual DateTime SysCreatedOn { get; protected set; }

        public virtual DateTime? SysModifiedOn { get; protected set; }

        public virtual bool IsDeleted { get; protected set; } = false;

        public virtual bool Delete()
        {
            if (this.IsDeleted)
            {
                return false;
            }

            this.IsDeleted = true;

            return this.IsDeleted;
        }

        public virtual void UpdateTrackValues()
        {
            this.SysModifiedOn = DateTime.UtcNow;
        }
    }
}

using UnityEngine;

namespace Wayfinder {
  public abstract class EntityState {

    protected Entity _entity;

    protected EntityState(Entity entity) {
      _entity = entity;
    }

    public virtual void OnStateEnter() { }

    public virtual void Execute() { }

    public virtual void OnStateExit() { }

    public virtual void OnTriggerEnter2D(Collider2D other) { }

    public virtual void OnTriggerExit2D(Collider2D other) { }
  }
}
using System;

namespace Wayfinder {
  [Serializable]
  public class AppliesTo {
    public bool enemies;
    public bool neutrals;
    public bool friendlies;
    public bool self;
    public bool environment;
  }
}
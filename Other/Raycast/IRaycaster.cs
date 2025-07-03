using System;

namespace CodeExamples.Codebase.InteractionSystem.Other.Raycast
{
    public interface IRaycaster
    {
        public ValueReferenceBase TargetReference { get; }
        public bool CastRay();
        public bool CastRayForAll();
        public bool HasTarget();
        public Type GetTargetType();
        public T GetRayCastResultAs<T>(bool clearResult = true) where T : class;
        public void Clear();
    }
}

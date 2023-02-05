using Mirror;
using UnityEngine;

namespace Sheep
{
    public class ThornController : NetworkBehaviour
    {
        [SerializeField] float pokeIntensity;
        [SerializeField] float randomIntensity;

        private void OnCollisionEnter(Collision collision)
        {
            if (!isServer) return;

            if (collision.collider.TryGetComponent(out SheepController sheep))
            {
                Vector2 randomFactor = Random.insideUnitCircle * randomIntensity;
                Vector3 randomDir = new Vector3(randomFactor.x, 0f, randomFactor.y);
                Vector3 dir = (sheep.transform.position - transform.position).normalized * pokeIntensity;
                dir = Vector3.ProjectOnPlane(dir, Vector3.up);
                sheep.AddMoveEffect(dir + randomDir);
            }
        }
    }
}

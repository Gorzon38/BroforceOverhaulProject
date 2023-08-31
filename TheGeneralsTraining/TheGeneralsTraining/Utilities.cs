using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TheGeneralsTraining
{
    public static class Utilities
    {
        public static bool SearchForOpenSpot(ref Vector3 pos, Vector3 direction)
        {
            direction.Normalize();
            Vector3 vector = pos;
            bool flag = false;
            int num = 0;
            while (num < 5 && !flag)
            {
                if (!Map.IsWithinBounds(pos) || Physics.CheckSphere(pos, 2f, Map.groundLayer))
                {
                    pos += Vector3.up * 16f;
                }
                else
                {
                    flag = true;
                }
                num++;
            }
            if (!flag)
            {
                pos = vector + direction * 16f + Vector3.up * 4f;
                for (int i = 0; i < 6; i++)
                {
                    if (!Map.IsWithinBounds(pos) || Physics.CheckSphere(pos, 2f, Map.groundLayer))
                    {
                        pos -= direction * 16f;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                RaycastHit raycastHit;
                if (Physics.Raycast(pos + Vector3.up, Vector3.down, out raycastHit, 32f, Map.groundLayer))
                {
                    pos = raycastHit.point;
                }
                return true;
            }
            pos = vector;
            return false;
        }

        public static bool IsOnAnimal(TestVanDammeAnim character)
        {
            LayerMask platformLayer = character.GetFieldValue<LayerMask>("platformLayer");
            RaycastHit raycastHit;
            return (Physics.Raycast(new Vector3(character.X, character.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer) ||
                Physics.Raycast(new Vector3(character.X + 4f, character.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer) ||
                Physics.Raycast(new Vector3(character.X - 4f, character.Y + 5f, 0f), Vector3.down, out raycastHit, 16f, platformLayer)) &&
                raycastHit.collider.GetComponentInParent<Animal>() != null;
        }
    }
}

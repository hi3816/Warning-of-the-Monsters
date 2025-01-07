using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class stationaryDefender : Monster //한자리를 지키고 있는 몬스터(=일반타워)
{
    void Update()
    {
        base.Update();
    }

    protected override void Scaring()
    {
        if (isSingleTargetAttack)
        {
            base.Scaring();
            return;
        }

        foreach (Human human in TargetHumanList)
        {
            if (human == null) continue;

            if (_lastScareTime >= data.cooldown)
            {
                MonsterProjectile projectile = PoolManager.Instance.SpawnFromPool<MonsterProjectile>(projectileData.projectileType.ToString(), transform.position, Quaternion.identity);
                projectile.SetProjectileInfo(human.transform.position, data, projectileData);
                projectile.gameObject.SetActive(true);

                _lastScareTime = 0f;
                SetState(MonsterState.Idle);
            }
        }

        if (TargetHumanList.Count == 0)
        {
            SetState(MonsterState.Idle);
        }
    }
}

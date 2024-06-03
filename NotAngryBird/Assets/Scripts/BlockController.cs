using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private float maxHp;

	protected float hp;
    public virtual float Hp
	{
		get => hp;
		set
		{
			hp = value;
			if (hp <= 0f)
			{
				//Transform childTransform = transform.GetChild(0);
				//childTransform.SetParent(null);
				//childTransform.gameObject.layer = LayerMask.NameToLayer("Enemy");
				//if (containsEnemy)
				//{
				//	childTransform.GetComponent<EnemyController>().Hp += hp; // ��� ������ �ʰ��� ��ŭ ü�� ����
				//}

				Destroy(gameObject);
			} else if (hp <= maxHp / 3f)
			{
				mySprite.sprite = almostBroken;
				print("something is almost broken");
			} else if (hp <= maxHp / 3f * 2f)
			{
				mySprite.sprite = slightlyBroken;
				print("something is slightly broken");
			} else
			{
				mySprite.sprite = notBroken;
			}
		}
	}

    public bool containsEnemy;

    private SpriteRenderer mySprite;

    [SerializeField]
    private Sprite almostBroken;

    [SerializeField]
    private Sprite slightlyBroken;

    [SerializeField]
    private Sprite notBroken;

    void Start()
    {
		mySprite = GetComponent<SpriteRenderer>();
		Hp = maxHp;
    }

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Rigidbody2D rb2d = collision.collider.GetComponent<Rigidbody2D>();
		float collisionForce = collisionForce = collision.relativeVelocity.magnitude;
		if (rb2d)
		{
			// �浹�� ���
			collisionForce /= (collision.collider.GetComponent<Rigidbody2D>().mass);
		}

		Hp -= collisionForce;
	}

    virtual protected void BeforeDestroy()
    {

    }

	private void OnDestroy()
	{
        BeforeDestroy();
	}
}

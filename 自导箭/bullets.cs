using UnityEngine;
using System.Collections;

public class bullets : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	public bool start;
	public GameObject aim;
	public float Damage;
	public float spCost;
	void OnDestroy()
	{
		try{
			if(Vector3 .Distance (this.transform .position,aim .transform .position)<0.1f)
			{
			peopleMain .shiledHp +=15+peopleMain .mofashuchu /10;
			int oneKiller = Random.Range (0, 1000);
			if (oneKiller < peopleMain .oneKill* 1000) 
			{
				aim.gameObject .GetComponent <monsterbasic > ().HP -= 
					aim.gameObject .GetComponent <monsterbasic > ().HPmax+100;
				if(	aim.gameObject .GetComponent <bloodLabelShower > ())
				{
					aim.gameObject .GetComponent <bloodLabelShower > ().LabelShowText ("斩!");
				}
				aim.gameObject .GetComponent <bloodLabelShower >().LabelShow (aim.gameObject .GetComponent <monsterbasic > ().HPmax);
				soundController .playSound(11);
			}
			else
			{
				lianji. attackLink();
				this.gameObject .audio .PlayOneShot (this.gameObject .audio .clip);
				float aimfakang = aim.GetComponent <monsterbasic > ().mofahujia;
				float truedamage = (Damage - peopleMain .mofachuantou) * (1 - aimfakang/2000) + peopleMain .mofachuantou;
				aim.GetComponent <monsterbasic > ().HP -= truedamage;

					calRead .damage +=truedamage;
					if(truedamage>calRead .danshang )
					{
						calRead .danshang =truedamage;
					}

					aim.gameObject .GetComponent <bloodLabelShower >().LabelShow (truedamage);
				//print ( "最大"+Damage +"伤害，实际"+ truedamage+"伤害");
					peopleMain.HP += truedamage * (0.075f+peopleMain .hpSpSuck);
					hpupLabel .skillHpUpShow(truedamage * (0.075f+peopleMain .hpSpSuck));
				
				if (aim.GetComponent <monsterbasic > ().HP < 0)
				{
					peopleMain .SP +=peopleMain .SPmax *0.07f;
				}
					jianyiBar .startCollect();
				}
			}
			else
			{
				peopleMain .SP +=spCost *0.3f;
			}
		
		}
		catch
		{
		}


	}
	void Update () {
		if (start) {
			Destroy(this.gameObject ,1f);
			transform.position=Vector3 .MoveTowards (this.transform.position, aim .transform.position, 2.15f);
			          if(Vector3 .Distance (this.transform .position,aim .transform .position)<0.1f)
			         {

				     Destroy (this.gameObject);
			         }
				}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager2 : Singleton<InteractionManager2>
{
    public List<GameObject> m_Animals;
    public List<MontureController> m_Saddles;

    public void AddAnimal(GameObject animal)
    {
        m_Animals.Add(animal);
    }

    public void AddSaddle(MontureController saddle)
    {
        m_Saddles.Add(saddle);
    }
}
    

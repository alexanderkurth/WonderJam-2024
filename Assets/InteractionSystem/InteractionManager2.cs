using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager2 : Singleton<InteractionManager2>
{
    public List<BaseIA> m_Animals = new List<BaseIA>();
    public List<MontureController> m_Saddles= new List<MontureController>();

    public void AddAnimal(BaseIA animal)
    {
        m_Animals.Add(animal);
    }

    public void AddSaddle(MontureController saddle)
    {
        m_Saddles.Add(saddle);
    }
}
    

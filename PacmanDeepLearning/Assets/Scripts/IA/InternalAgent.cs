using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class InternalAgent : Agent {

    public class Replay
    {
        public List<double> states;
        public double reward;

        public Replay(List<float> s, double r)
        {
            states = new List<double>();
            foreach(float st in s){
                states.Add((double)st);
            }
            reward = r;
        }
    }

    ANN ann;
    float reward = 0.0f;							//reward to associate with actions
	List<Replay> replayMemory = new List<Replay>();	//memory - list of past actions and rewards
	int mCapacity = 2000;							//memory capacity
    List<double> qs = new List<double>();
	
	float discount = 0.99f;							//how much future states affect rewards
	float maxExploreRate = 100.0f;					//max chance value
    float minExploreRate = 0.01f;					//min chance value
    float exploreDecay = 0.0001f;					//chance decay amount for each update

	public float[][] q_table;	// The matrix containing the values estimates.
	float learning_rate = 0.5f;	// The rate at which to update the value estimates given a reward.
	int action = -1;
    float gamma = 0.99f; // Discount factor for calculating Q-target.
    float e = 1; // Initial epsilon value for random action selection.
    float eMin = 0.2f; // Lower bound of epsilon.
    int annealingSteps = 5000; // Number of steps to lower e to eMin.
    List<float> lastState;

	public override void SendParameters (EnvironmentParameters env)
	{
        ann = new ANN(10,4,4,4,0.2f);
	}

	/// <summary>
    /// Picks an action to take from its current state.
	/// </summary>
	/// <returns>The action choosen by the agent's policy</returns>
	public override float[] GetAction(List<float> statec) {
        List<double> states = new List<double>();
        foreach(float state in statec){
            states.Add((double)state);
        }
        qs = SoftMax(ann.CalcOutput(states));
		double maxQ = qs.Max();
		action = qs.ToList().IndexOf(maxQ);
        
        if (Random.Range(0f, 1f) < e) { action = Random.Range(0, 4); }
        if (e > eMin) { e = e - ((1f - eMin) / (float)annealingSteps); }

		return new float[1] {action};
	}

    /*/// <summary>
    /// Gets the values stored within the Q table.
    /// </summary>
    /// <returns>The average Q-values per state.</returns>
	public override float[] GetValue() {
        for (int i = 0; i < qs.Length; i++)
        {
            value_table[i] = q_table[i].Average();
        }
		return value_table;
	}*/

    /// <summary>
    /// Updates the value estimate matrix given a new experience (state, action, reward).
    /// </summary>
    /// <param name="state">The environment state the experience happened in.</param>
    /// <param name="reward">The reward recieved by the agent from the environment for it's action.</param>
    /// <param name="done">Whether the episode has ended</param>
    public override void SendState(List<float> state, float reward, bool done)
    {
        if(reward == 1)
            Debug.Log("won");

        Replay lastMemory = new Replay(state, reward);

		if(replayMemory.Count > mCapacity)
			replayMemory.RemoveAt(0);
		
		replayMemory.Add(lastMemory);
	}

    public override void Train(){
        for(int i = replayMemory.Count - 1; i >= 0; i--)
        {
            List<double> toutputsOld = new List<double>();
            List<double> toutputsNew = new List<double>();
            toutputsOld = SoftMax(ann.CalcOutput(replayMemory[i].states));	

            double maxQOld = toutputsOld.Max();
            int action = toutputsOld.ToList().IndexOf(maxQOld);

            double feedback;
            if(i == replayMemory.Count-1 || replayMemory[i].reward == -1)
                feedback = replayMemory[i].reward;
            else
            {
                toutputsNew = SoftMax(ann.CalcOutput(replayMemory[i+1].states));
                double maxQ = toutputsNew.Max();
                feedback = (replayMemory[i].reward + 
                    discount * maxQ);
            } 

            toutputsOld[action] = feedback;
            ann.Train(replayMemory[i].states,toutputsOld);
        }
        //Debug.Log(ann.PrintWeights());
    }

    List<double> SoftMax(List<double> values) 
    {
      double max = values.Max();

      float scale = 0.0f;
      for (int i = 0; i < values.Count; ++i)
        scale += Mathf.Exp((float)(values[i] - max));

      List<double> result = new List<double>();
      for (int i = 0; i < values.Count; ++i)
        result.Add(Mathf.Exp((float)(values[i] - max)) / scale);

      return result; 
    }
}

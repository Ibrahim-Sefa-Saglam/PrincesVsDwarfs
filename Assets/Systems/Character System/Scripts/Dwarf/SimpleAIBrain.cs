using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAIBrain : MonoBehaviour
{
    [System.Serializable]
    public struct RayDetectionData
    {
        public float length;
        public Vector2 hitPosition;
        public GameObject hitObject;
        public string tag;
        public int layer;
        public float angle;
    }
    public List<RayDetectionData> detectionTable = new List<RayDetectionData>();

    public class EvaluationResult
    {
        public string actionName;
        public float rawScore;
        public float probability;

        public EvaluationResult(string action, float score)
        {
            actionName = action;
            rawScore = score;
            probability = 0f;
        }
    }

    private Dictionary<string, EvaluationResult> evaluationTable = new Dictionary<string, EvaluationResult>();

    [System.Serializable]
    public class ActionActivations
    {
        public bool MoveForwardIsActive;
        public bool JumpIsActive;
        public bool FireIsActive;
        public bool TurnIsActive;
    }
    
    [Header("Action Activity Flags")]
    public ActionActivations actionStates = new ActionActivations();
    
    [Range(0f, 1f)]
    public float resultThreshold = 0.25f;

    public float evaluationPeriod = 1f; // How often to evaluate in seconds
    private Coroutine evaluationRoutine;

    
    void Start()
    {
        evaluationRoutine = StartCoroutine(EvaluateRoutine());
    }
    
    // === Layer 1 Evaluation Methods ===

    private void EvaluateMoveForward()
    {
        float score = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.hitObject == null)
                score += 1f;
            else if (ray.length > 1.5f)
                score += 0.5f;
            else
                score -= 1f;
        }

        evaluationTable["MoveForward"] = new EvaluationResult("MoveForward", Mathf.Max(0, score));
    }

    private void EvaluateJump() // creates a score based on the detection table data and saves the score to evaluation table with the action name
    {
        float score = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.angle > -10f && ray.angle < 10f && ray.length < 1f && ray.hitObject)
                score += 2f;

            if (ray.angle > 30f && ray.tag == "Platform")
                score += 1f;

            if (ray.angle > 30f && ray.tag == "Enemy")
                score += 1.5f;
        }

        evaluationTable["Jump"] = new EvaluationResult("Jump", Mathf.Max(0, score));
    }

    private void EvaluateFire()
    {
        float score = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.tag == "Enemy")
            {
                float alignment = 1f - Mathf.Abs(ray.angle) / 45f;
                float proximity = Mathf.Clamp(2f - ray.length, 0f, 2f);
                score += alignment * proximity;
            }
        }

        evaluationTable["Fire"] = new EvaluationResult("Fire", Mathf.Max(0, score));
    }

    private void EvaluateTurn()
    {
        float score = 0f;
        float forwardBlock = 0f;
        float sideOpen = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.angle > -10f && ray.angle < 10f && ray.length < 1f)
                forwardBlock += 1f;

            if (Mathf.Abs(ray.angle) > 30f && ray.length > 2f)
                sideOpen += 1f;
        }

        score = forwardBlock + sideOpen;

        evaluationTable["Turn"] = new EvaluationResult("Turn", Mathf.Max(0, score));
    }

    // compares every action's score to the sum of all scores in the evaluation Table:   (individual score probability) = (individual action score) / (total score)
    // SUMMARY: Turns the evaluation scores into probabilities 
    private void NormalizeScoresToProbabilities()
    {
        float total = 0f;
        foreach (var eval in evaluationTable.Values)
            total += eval.rawScore;

        if (total <= 0f) total = 1f;

        foreach (var eval in evaluationTable.Values)
            eval.probability = eval.rawScore / total;
    }

    // returns those that pass the threshold 
    public List<string> GetDecisions(float threshold)
    {
        List<string> decisions = new List<string>();

        foreach (var eval in evaluationTable.Values)
        {
            if (eval.probability >= threshold)
            {
                decisions.Add(eval.actionName);
            }
        }

        return decisions;
    }

    private IEnumerator EvaluateRoutine()
    {
        List<string> aiResults;
        while (true)
        {
            aiResults = RunAI(); // Perform evaluation
         
            string log = "AI Decisions: ";
            foreach (var action in aiResults)
            {
                if (evaluationTable.TryGetValue(action, out var result))
                {
                    log += $"{action} ({result.probability:F2})  ";
                }
            }

            Debug.Log(log);


            yield return new WaitForSeconds(evaluationPeriod); 
        }
    }
    public List<string> RunAI()
    {
        evaluationTable.Clear();
        
        if(actionStates.MoveForwardIsActive) EvaluateMoveForward();
        if(actionStates.JumpIsActive)EvaluateJump();
        if(actionStates.FireIsActive)EvaluateFire();
        if(actionStates.TurnIsActive)EvaluateTurn();

        NormalizeScoresToProbabilities();

        return GetDecisions(resultThreshold);
    }
    
}



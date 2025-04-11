using System.Collections.Generic;
using UnityEngine;

public class SimpleAIBrain : MonoBehaviour
{
    // === Detection Table ===
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

    // === Evaluation Table (Layer 1 Output) ===
    public class EvaluationResult
    {
        public string actionName;
        public float score;

        public EvaluationResult(string action, float value)
        {
            actionName = action;
            score = value;
        }
    }

    private List<EvaluationResult> evaluationTable = new List<EvaluationResult>();

    // === Threshold for Result Evaluation ===
    public float resultThreshold = 1f;

    // === Layer 1 Evaluation Methods ===

    public void EvaluateMoveForward()
    {
        float score = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.hitObject == null)
            {
                score += 1f;
            }
            else if (ray.length > 1.5f)
            {
                score += 0.5f;
            }
            else
            {
                score -= 1f;
            }
        }

        evaluationTable.Add(new EvaluationResult("MoveForward", score));
    }

    public void EvaluateJump()
    {
        float score = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.angle > -10f && ray.angle < 10f && ray.length < 1f && ray.hitObject != null)
            {
                score += 2f;
            }

            if (ray.angle > 30f && ray.tag == "Platform")
            {
                score += 1f;
            }

            if (ray.angle > 30f && ray.tag == "Enemy")
            {
                score += 1.5f;
            }
        }

        evaluationTable.Add(new EvaluationResult("Jump", score));
    }

    public void EvaluateFire()
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

        evaluationTable.Add(new EvaluationResult("Fire", score));
    }

    public void EvaluateTurn()
    {
        float score = 0f;
        float forwardBlock = 0f;
        float sideOpen = 0f;

        foreach (var ray in detectionTable)
        {
            if (ray.angle > -10f && ray.angle < 10f && ray.length < 1f)
            {
                forwardBlock += 1f;
            }

            if (Mathf.Abs(ray.angle) > 30f && ray.length > 2f)
            {
                sideOpen += 1f;
            }
        }

        score = forwardBlock + sideOpen;

        evaluationTable.Add(new EvaluationResult("Turn", score));
    }

    // === Result Method (Layer 3) ===
    // Returns the string component of the elements in evaluation table with score >= treshold  
    public List<string> GetDecisions(float threshold)
    {
        List<string> decisions = new List<string>();

        foreach (var eval in evaluationTable)
        {
            if (eval.score >= threshold)
            {
                decisions.Add(eval.actionName);
            }
        }

        return decisions;
    }

    // === Public Entry Point ===
    public List<string> RunAI()
    {
        evaluationTable.Clear(); // Reset evaluation results

        EvaluateMoveForward();
        EvaluateJump();
        EvaluateFire();
        EvaluateTurn();

        return GetDecisions(resultThreshold);
    }
}

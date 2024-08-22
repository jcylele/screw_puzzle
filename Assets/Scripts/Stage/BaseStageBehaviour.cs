namespace Stage
{
    public abstract class BaseStageBehaviour : BaseBehaviour
    {
        public StageInfo stageInfo;

        public abstract void ExpandStage();

        public abstract void ClearStage();
    }
}
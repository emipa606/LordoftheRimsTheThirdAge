using System.Xml;
using JetBrains.Annotations;
using Verse;

namespace TheThirdAge
{
    [UsedImplicitly]
    public class PatchOperationRemoveModernStuff : PatchOperation
    {
        /*
        private static readonly PatchOperationRemove removeOperation = new PatchOperationRemove();
        private static readonly Traverse setXpathTraverse = Traverse.Create(root: removeOperation).Field(name: "xpath");
        private static readonly string xpath = $"//techLevel[.='{string.Join(separator: "' or .='", value: Enum.GetValues(enumType: typeof(TechLevel)).Cast<TechLevel>().Where(predicate: tl => tl > RemoveModernStuff.MAX_TECHLEVEL).Select(selector: tl => tl.ToString()).ToArray())}']/..";
        */
        protected override bool ApplyWorker(XmlDocument xml)
        {
            /*
            setXpathTraverse.SetValue(value: xpath);
            removeOperation.Apply(xml: xml);
            */
            return true;
        }
    }
}
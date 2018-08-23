using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot ("dialogue")]
public class Dialogue
{
    [XmlAttribute("id")]
    public string id;

    [XmlAttribute("cond")]
    public string cond;

    [XmlElement("conversation")]
    public List<Conversation> conversation;

    [System.Serializable]
    public class Conversation
    {
        [XmlElement("relationsMin")]
        public float relationsMin = -100.0f;

        [XmlElement("relationsMax")]
        public float relationsMax = 100.0f;

        [XmlArray("party"), XmlArrayItem("member")]
        public List<string> party;

        [XmlArray("vars"), XmlArrayItem("var")]
        public List<string> vars;

        [XmlArray("dialogueChains"), XmlArrayItem("chain")]
        public List<Chain> dialogueChains;

        [System.Serializable]
        public class Chain
        {
            [XmlAttribute("repeaterLines")]
            public int repeaterLines = 1;

            [XmlElement("dialogueOptions")]
            public List<DialogueOptions> dialogueOptions;

            [System.Serializable]
            public class DialogueOptions
            {
                [XmlAttribute("id")]
                public string id = "";

                [XmlAttribute("speaker")]
                public string speaker;

                [XmlAttribute("type")]
                public string type = "";

                [XmlElement("textOptions")]
                public List<TextOptions> textOptions;

                [XmlElement("choiceOptions")]
                public ChoiceOptions choiceOptions = null;

                [XmlIgnore]
                public bool used = false;

                [System.Serializable]
                public class TextOptions
                {
                    [XmlAttribute("persona")]
                    public string persona = "None";

                    [XmlAttribute("rationalMin")]
                    public float rationalMin = -100.0f;

                    [XmlAttribute("rationalMax")]
                    public float rationalMax = 100.0f;

                    [XmlAttribute("seriousMin")]
                    public float seriousMin = -100.0f;

                    [XmlAttribute("seriousMax")]
                    public float seriousMax = 100.0f;

                    [XmlAttribute("portrait")]
                    public string portraitValue = "0";

                    [XmlElement("text")]
                    public List<DialogueText> dialogueText;

                    [System.Serializable]
                    public class DialogueText
                    {
                        [XmlText]
                        public string text;
                    }
                }

                [System.Serializable]
                public class ChoiceOptions
                {
                    [XmlAttribute("chooser")]
                    public string chooser = "player";

                    [XmlElement("choice")]
                    public List<Choice> choice;

                    [System.Serializable]
                    public class Choice
                    {
                        [XmlAttribute("id")]
                        public string id;

                        [XmlAttribute("persona")]
                        public string persona = "None";

                        [XmlElement("select")]
                        public string select;

                        [XmlElement("textOptions")]
                        public List<TextOptions> textOptions;
                    }
                }
            }
        }
    }
}

public class ConversationRestriction
{
    public float relationsMin = -100.0f;
    public float relationsMax = 100.0f;
    public List<string> party;
    public List<string> vars;

    // Class constructors
    public ConversationRestriction(float relMin, float relMax, List<string> resParty, List<string> resVars)
    {
        relationsMin = relMin;
        relationsMax = relMax;
        party = resParty;
        vars = resVars;
    }
    public ConversationRestriction(Dialogue.Conversation conversation)
    {
        relationsMin = conversation.relationsMin;
        relationsMax = conversation.relationsMax;
        party = conversation.party;
        vars = conversation.vars;
    }

    // Override how equals operator works for this object. Used to compare against Dialogue object
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is ConversationRestriction)
        {
            ConversationRestriction c = obj as ConversationRestriction;

            if (c.relationsMin.Equals(relationsMin) && c.relationsMax.Equals(relationsMax)
                && compareLists(c.party, party) && compareLists(c.vars, vars))
                return true;
        }

        if (obj is Dialogue.Conversation)
        {
            Dialogue.Conversation dc = obj as Dialogue.Conversation;

            if (dc.relationsMin.Equals(relationsMin) && dc.relationsMax.Equals(relationsMax)
                && compareLists(dc.party, party) && compareLists(dc.vars, vars))
                return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    // Returns true if two lists of strings match each other
    public bool compareLists(List<string> first, List<string> second)
    {
        if (first == null && second == null)
            return true;
        if (first.Count != second.Count)
            return false;

        for (int i = 0; i < first.Count; i++)
            if (first[i] != second[i])
                return false;

        return true;
    }
}

public class TextOptionRestriction
{
    public float rationalMin = -100.0f;
    public float rationalMax = 100.0f;
    public float seriousMin = -100.0f;
    public float seriousMax = 100.0f;

    // Class constructors
    public TextOptionRestriction (float ratMin, float ratMax, float serMin, float serMax)
    {
        rationalMin = ratMin;
        rationalMax = ratMax;
        seriousMin = serMin;
        seriousMax = serMax;
    }
    public TextOptionRestriction (Dialogue.Conversation.Chain.DialogueOptions.TextOptions textOption)
    {
        rationalMin = textOption.rationalMin;
        rationalMax = textOption.rationalMax;
        seriousMin = textOption.seriousMin;
        seriousMax = textOption.seriousMax;
    }

    // Override how equals operator works for this object
    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;

        if (obj is TextOptionRestriction)
        {
            TextOptionRestriction t = obj as TextOptionRestriction;

            if (t.rationalMin.Equals(rationalMin) && t.rationalMax.Equals(rationalMax)
                && t.seriousMin.Equals(seriousMin) && t.seriousMax.Equals(seriousMax))
                return true;
        }

        if (obj is Dialogue.Conversation.Chain.DialogueOptions.TextOptions)
        {
            Dialogue.Conversation.Chain.DialogueOptions.TextOptions dt = obj as Dialogue.Conversation.Chain.DialogueOptions.TextOptions;

            if (dt.rationalMin.Equals(rationalMin) && dt.rationalMax.Equals(rationalMax)
                && dt.seriousMin.Equals(seriousMin) && dt.seriousMax.Equals(seriousMax))
                return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
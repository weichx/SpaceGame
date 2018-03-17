namespace Weichx.Persistence {

    public struct StringReader {

        private string line;
        private int pointer;

        public StringReader(string line) {
            this.line = line;
            this.pointer = 0;
        }

        public int ReadTaggedInt(char tag) {
            AdvanceToTag();
            if (line[pointer] == tag && pointer + 1 < line.Length && line[pointer + 1] != ' ') {
                pointer++;
                return ReadInt();
            }
            return -1;
        }

        public int ReadInt() {
            int start = pointer;
            for (; pointer < line.Length; pointer++) {
                if (line[pointer] == ' ') {
                    pointer++;
                    return int.Parse(line.Substring(start, pointer - start - 1));
                }
            }
            string str = line.Substring(start, pointer - start);
            return int.Parse(str);
        }

        public float ReadFloat() {
            int start = pointer;
            for (; pointer < line.Length; pointer++) {
                if (line[pointer] == ' ') {
                    pointer++;
                    return float.Parse(line.Substring(start, pointer - start - 1));
                }
            }
            string str = line.Substring(start, pointer - start);
            return float.Parse(str);
        }

        public string ReadString() {
            int start = pointer;
            for (; pointer < line.Length; pointer++) {
                if (line[pointer] == ' ') {
                    pointer++;
                    return line.Substring(start, pointer - start - 1);
                }
            }
            return line.Substring(start, pointer - start);
        }

        public string ReadLine() {
            return line.Substring(pointer);
        }

        public void AdvanceToTag() {
            for (; pointer < line.Length; pointer++) {
                if (line[pointer] == '@') break;
            }
            pointer++;
        }

        public void Reset() {
            pointer = 0;
        }

    }

}
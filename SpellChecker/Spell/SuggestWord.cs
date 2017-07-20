/* 
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace SpellChecker.Net.Search.Spell
{
	
    /// <summary>  SuggestWord Class, used in suggestSimilar method in SpellChecker class.
    /// 
    /// </summary>
    /// <author>  Nicolas Maisonneuve
    /// </author>
    sealed class SuggestWord
    {
        /// <summary> the score of the word</summary>
        public float score;
		
        /// <summary> The freq of the word</summary>
        public int freq;
		
        /// <summary> the suggested word</summary>
        public System.String termString;
		
        public int CompareTo(SuggestWord a)
        {
            //first criteria: the edit distance
            if (score > a.score)
            {
                return 1;
            }
            if (score < a.score)
            {
                return - 1;
            }
			
            //second criteria (if first criteria is equal): the popularity
            if (freq > a.freq)
            {
                return 1;
            }
			
            if (freq < a.freq)
            {
                return - 1;
            }
			
            return 0;
        }
    }
}
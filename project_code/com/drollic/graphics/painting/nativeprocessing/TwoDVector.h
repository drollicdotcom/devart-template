// FastRawImage.h
#pragma once

/*
Copyright © 2006, Drollic
All rights reserved.
http://www.drollic.com

Redistribution of this software in source or binary forms, with or 
without modification, is expressly prohibited. You may not reverse-assemble, 
reverse-compile, or otherwise reverse-engineer this software in any way.

THIS SOFTWARE ("SOFTWARE") IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
*/

#include <vector>

namespace com
{
	namespace drollic
	{
		namespace graphics
		{
			namespace painting
			{
				namespace native
				{
					namespace processing
					{
						using namespace std;

						template <class T>
						class TwoDVector
						{
						public:
							TwoDVector():m_dimRow(0), m_dimCol(0){;}
							TwoDVector(int nRow, int nCol) {
								m_dimRow = nRow;
								m_dimCol = nCol;
								for (int i=0; i < nRow; i++){
									vector<T> x(nCol);
									int y = x.size();
									m_2DVector.push_back(x);
								}
							}
							inline void SetAt(int nRow, int nCol, const T& value) {
								if(nRow >= m_dimRow || nCol >= m_dimCol)
									throw out_of_range("Array out of bound");
								else
									m_2DVector[nRow][nCol] = value;
							}
							inline T GetAt(int nRow, int nCol) {
								if(nRow >= m_dimRow || nCol >= m_dimCol)
									throw out_of_range("Array out of bound");
								else
									return m_2DVector[nRow][nCol];
							}
							void GrowRow(int newSize) {
								if (newSize <= m_dimRow)
									return;
								m_dimRow = newSize;
								for(int i = 0 ; i < newSize - m_dimCol; i++)   {
									vector<int> x(m_dimRow);
									m_2DVector.push_back(x);
								}
							}
							void GrowCol(int newSize) {
								if(newSize <= m_dimCol)
									return;
								m_dimCol = newSize;
								for (int i=0; i <m_dimRow; i++)
									m_2DVector[i].resize(newSize);
							}
							vector<T>& operator[](int x)    {
								return m_2DVector[x];
							}
						private:
							vector< vector <T> > m_2DVector;
							int m_dimRow;
							int m_dimCol;
						};
					}
				}
			}
		}
	}
}
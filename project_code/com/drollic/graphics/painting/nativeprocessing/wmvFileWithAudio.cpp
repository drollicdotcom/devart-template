#include "StdAfx.h"
#include "wmvfileWithAudio.h"

#include <comdef.h>    // Compiler COM support
#include <atlbase.h>

#ifndef __countof
#define __countof(x)	((sizeof(x)/sizeof(x[0])))
#endif

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
						CwmvFileWithAudio::	CwmvFileWithAudio(
							const GUID& guidProfileID /*= WMProfile_V80_384Video*/,
							DWORD dwFrameRate /*= 1*/)

						{
							LPCTSTR lpszFileName = _T("Output.wmv");
							CoInitialize(NULL);

							HRESULT	hr=E_FAIL;
							GUID    guidInputType;
							DWORD	dwInputCount=0;
							IWMInputMediaProps* pVideoInputProps = NULL;
							IWMInputMediaProps* pAudioInputProps = NULL;
							IWMProfileManager2	*pProfileManager2=NULL;

							m_hwmvDC				= NULL;
							m_pVideoWMWriter				= NULL;
							m_pAudioWMWriter				= NULL;
							m_dwVideoInput			= 0;
							m_dwAudioInput			= 0;
							m_msVideoTime			= 0;
							m_pWMProfile			= NULL;
							m_pVideoProps			= NULL;
							m_pAudioProps			= NULL;
							m_pWMProfileManager		= NULL;
							m_dwCurrentVideoSample	= 0;
							m_dwFrameRate			= dwFrameRate;

							_tcscpy(m_szErrMsg, _T("Method Succeeded"));
							m_szErrMsg[__countof(m_szErrMsg)-1] = _T('\0');

							pAppendFrame[0]		= &CwmvFileWithAudio::AppendDummy; // VC8 requires & for Function Pointer; Remove it if your compiler complains;
							pAppendFrame[1]		= &CwmvFileWithAudio::AppendFrameFirstTime;
							pAppendFrame[2]		= &CwmvFileWithAudio::AppendFrameUsual;

							pAppendFrameBits[0]	= &CwmvFileWithAudio::AppendDummy;
							pAppendFrameBits[1]	= &CwmvFileWithAudio::AppendFrameFirstTime;
							pAppendFrameBits[2]	= &CwmvFileWithAudio::AppendFrameUsual;

							m_nAppendFuncSelector=0;		//Point to DummyFunction

							if(FAILED(WMCreateProfileManager(&m_pWMProfileManager)))
							{
								SetErrorMessage(L"Unable to Create WindowsMedia Profile Manager");
								goto TerminateConstructor;
							}

							if(FAILED(m_pWMProfileManager->QueryInterface(IID_IWMProfileManager2,(void**)&pProfileManager2)))
							{
								SetErrorMessage(L"Unable to Query Interface for ProfileManager2");
								goto TerminateConstructor;
							}

							hr=pProfileManager2->SetSystemProfileVersion(WMFORMAT_SDK_VERSION);

							pProfileManager2->Release();

							if(FAILED(hr))
							{
								SetErrorMessage(L"Unable to Set System Profile Version");
								goto TerminateConstructor;
							}

							if(FAILED(m_pWMProfileManager->LoadProfileByID(guidProfileID,&m_pWMProfile)))
							{
								SetErrorMessage(L"Unable to Load System Profile");
								goto TerminateConstructor;
							}

							if(FAILED(WMCreateWriter(NULL,&m_pVideoWMWriter)))
							{
								SetErrorMessage(L"Unable to Create Media Writer Object");
								goto TerminateConstructor;
							}

							if(FAILED(WMCreateWriter(NULL,&m_pAudioWMWriter)))
							{
								SetErrorMessage(L"Unable to Create Media Writer Object");
								goto TerminateConstructor;
							}

							if(FAILED(m_pVideoWMWriter->SetProfile(m_pWMProfile)))
							{
								SetErrorMessage(L"Unable to Set System Profile");
								goto TerminateConstructor;
							}

							if(FAILED(m_pAudioWMWriter->SetProfile(m_pWMProfile)))
							{
								SetErrorMessage(L"Unable to Set System Profile");
								goto TerminateConstructor;
							}

							if(FAILED(m_pVideoWMWriter->GetInputCount(&dwInputCount)))
							{
								SetErrorMessage(L"Unable to Get input count For Profile");
								goto TerminateConstructor;
							}

							for(DWORD	i=0;i<dwInputCount;i++)
							{
								if(FAILED(m_pVideoWMWriter->GetInputProps(i,&pVideoInputProps)))
								{
									SetErrorMessage(L"Unable to GetInput Properties");
									goto TerminateConstructor;
								}
								if(FAILED(pVideoInputProps->GetType(&guidInputType)))
								{
									SetErrorMessage(L"Unable to Get Input Property Type");
									goto TerminateConstructor;
								}
								if(guidInputType==WMMEDIATYPE_Video)
								{
									m_pVideoProps=pVideoInputProps;
									m_dwVideoInput=i;
									break;
								}
								else
								{
									pVideoInputProps->Release();
									pVideoInputProps=NULL;
								}
							}

							if(m_pVideoProps==NULL)
							{
								SetErrorMessage(L"Profile Does not Accept Video input");
								goto TerminateConstructor;
							}


							if(FAILED(m_pAudioWMWriter->GetInputCount(&dwInputCount)))
							{
								SetErrorMessage(L"Unable to Get input count For Profile");
								goto TerminateConstructor;
							}

							for(DWORD	i=0;i<dwInputCount;i++)
							{
								if(FAILED(m_pAudioWMWriter->GetInputProps(i,&pAudioInputProps)))
								{
									SetErrorMessage(L"Unable to GetInput Properties");
									goto TerminateConstructor;
								}
								if(FAILED(pAudioInputProps->GetType(&guidInputType)))
								{
									SetErrorMessage(L"Unable to Get Input Property Type");
									goto TerminateConstructor;
								}
								if(guidInputType==WMMEDIATYPE_Audio)
								{
									m_pAudioProps=pAudioInputProps;
									m_dwAudioInput=i;
									break;
								}
								else
								{
									pAudioInputProps->Release();
									pAudioInputProps=NULL;
								}
							}

							if(m_pVideoProps==NULL)
							{
								SetErrorMessage(L"Profile Does not Accept Video input");
								goto TerminateConstructor;
							}

#ifndef UNICODE
							WCHAR       pwszOutFile[1024];
							if( 0 == MultiByteToWideChar( CP_ACP, 0, lpszFileName,-1, pwszOutFile, sizeof( pwszOutFile ) ) )
							{
								SetErrorMessage("Unable to Convert Output Filename");
								goto TerminateConstructor;
							}
							if(FAILED(m_pWMWriter->SetOutputFilename( pwszOutFile )))
							{
								SetErrorMessage("Unable to Set Output Filename");
								goto TerminateConstructor;
							}
#else
							if(FAILED(m_pVideoWMWriter->SetOutputFilename(lpszFileName)))
							{
								SetErrorMessage(L"Unable to Set Output Filename");
								goto TerminateConstructor;
							}

							if(FAILED(m_pAudioWMWriter->SetOutputFilename(lpszFileName)))
							{
								SetErrorMessage(L"Unable to Set Output Filename");
								goto TerminateConstructor;
							}

#endif	//UNICODE

							m_nAppendFuncSelector=1;		//0=Dummy	1=FirstTime	2=Usual

							return;

TerminateConstructor:

							ReleaseMemory();

							return;
						}

						CwmvFileWithAudio::~CwmvFileWithAudio(void)
						{
							ReleaseMemory();

							CoUninitialize();
						}

						void CwmvFileWithAudio::ReleaseMemory()
						{
							if(m_nAppendFuncSelector==2)	//If we are Currently Writing 
							{
								if(m_pVideoWMWriter)
									m_pVideoWMWriter->EndWriting();	

								if(m_pAudioWMWriter)
									m_pAudioWMWriter->EndWriting();	

							}

							m_nAppendFuncSelector=0;		//Point to DummyFunction

							if(m_pVideoProps)
							{
								m_pVideoProps->Release();
								m_pVideoProps=NULL;
							}
							if(m_pAudioProps)
							{
								m_pAudioProps->Release();
								m_pAudioProps=NULL;
							}

							if(m_pVideoWMWriter)
							{
								m_pVideoWMWriter->Release();
								m_pVideoWMWriter=NULL;
							}
							if(m_pAudioWMWriter)
							{
								m_pAudioWMWriter->Release();
								m_pAudioWMWriter=NULL;
							}

							if(m_pWMProfile)
							{
								m_pWMProfile->Release();
								m_pWMProfile=NULL;
							}
							if(m_pWMProfileManager)
							{
								m_pWMProfileManager->Release();
								m_pWMProfileManager=NULL;
							}
							if(m_hwmvDC)
							{
								DeleteDC(m_hwmvDC);
								m_hwmvDC=NULL;
							}
						}

						void CwmvFileWithAudio::SetErrorMessage(LPCTSTR lpszErrorMessage)
						{
							_tcsncpy(m_szErrMsg, lpszErrorMessage, __countof(m_szErrMsg)-1);
						}

						BSTR GetErrorMessage(_com_error e)
						{
							CComBSTR bstrMsg(_T(""));
							TCHAR tmpStr[512];

							_sntprintf(tmpStr, sizeof(tmpStr), _T("COM Error HRESULT 0x%X\n"), e.Error());
							bstrMsg.Append(tmpStr);

							_sntprintf(tmpStr, sizeof(tmpStr), _T("_com_error::ErrorMessage() = %s\n"), e.ErrorMessage());
							bstrMsg.Append(tmpStr);

							IErrorInfo *ptrIErrorInfo = e.ErrorInfo();
							if (ptrIErrorInfo != NULL)
							{
								// IErrorInfo Interface located

								_sntprintf(tmpStr, sizeof(tmpStr), _T("_com_error:escription() = %s\n"), (TCHAR *)e.Description());
								bstrMsg.Append(tmpStr);

								_sntprintf(tmpStr, sizeof(tmpStr), _T("\n_com_error::Source() = %s"), (TCHAR *)e.Source());
								bstrMsg.Append(tmpStr);
							}
							else
							{
								bstrMsg.Append("No IErrorInfo Interface present\n");
							}

							// implicit BSTR cast operator used
							return(bstrMsg);
						}

						HRESULT CwmvFileWithAudio::InitMovieCreation(int nFrameWidth, int nFrameHeight, int nBitsPerPixel)
						{
							int	nMaxWidth=GetSystemMetrics(SM_CXSCREEN), nMaxHeight=GetSystemMetrics(SM_CYSCREEN);

							BITMAPINFO	bmpInfo;

							m_hwmvDC=CreateCompatibleDC(NULL);
							if(m_hwmvDC==NULL)	
							{
								SetErrorMessage(L"Unable to Create Device Context");
								return E_FAIL;
							}

							ZeroMemory(&bmpInfo,sizeof(BITMAPINFO));
							bmpInfo.bmiHeader.biSize		= sizeof(BITMAPINFOHEADER);
							bmpInfo.bmiHeader.biBitCount	= nBitsPerPixel;	
							bmpInfo.bmiHeader.biWidth		= nFrameWidth;
							bmpInfo.bmiHeader.biHeight		= nFrameHeight;
							bmpInfo.bmiHeader.biPlanes		= 1;
							bmpInfo.bmiHeader.biSizeImage	= nFrameWidth*nFrameHeight*nBitsPerPixel/8;
							bmpInfo.bmiHeader.biCompression	= BI_RGB;

							if(bmpInfo.bmiHeader.biHeight>nMaxHeight)	nMaxHeight=bmpInfo.bmiHeader.biHeight;
							if(bmpInfo.bmiHeader.biWidth>nMaxWidth)	nMaxWidth=bmpInfo.bmiHeader.biWidth;

							WMVIDEOINFOHEADER	videoInfo;
							videoInfo.rcSource.left		= 0;
							videoInfo.rcSource.top		= 0;
							videoInfo.rcSource.right	= bmpInfo.bmiHeader.biWidth;
							videoInfo.rcSource.bottom	= bmpInfo.bmiHeader.biHeight;
							videoInfo.rcTarget			= videoInfo.rcSource;
							videoInfo.rcTarget.right	= videoInfo.rcSource.right;
							videoInfo.rcTarget.bottom	= videoInfo.rcSource.bottom;
							videoInfo.dwBitRate= (nMaxWidth*nMaxHeight*bmpInfo.bmiHeader.biBitCount* m_dwFrameRate);
							videoInfo.dwBitErrorRate	= 0;
							//videoInfo.AvgTimePerFrame	= ((QWORD)1) * 10000 * 1000 / m_dwFrameRate;
							videoInfo.AvgTimePerFrame	= ((QWORD)1) / m_dwFrameRate;
							memcpy(&(videoInfo.bmiHeader),&bmpInfo.bmiHeader,sizeof(BITMAPINFOHEADER));

							WM_MEDIA_TYPE mt;
							mt.majortype = WMMEDIATYPE_Video;
							if( bmpInfo.bmiHeader.biCompression == BI_RGB )
							{
								if( bmpInfo.bmiHeader.biBitCount == 32 )
								{
									mt.subtype = WMMEDIASUBTYPE_RGB32;
								} 
								else if( bmpInfo.bmiHeader.biBitCount == 24 )
								{
									mt.subtype = WMMEDIASUBTYPE_RGB24;
								}
								else if( bmpInfo.bmiHeader.biBitCount == 16 )
								{
									mt.subtype = WMMEDIASUBTYPE_RGB555;
								}
								else if( bmpInfo.bmiHeader.biBitCount == 8 )
								{
									mt.subtype = WMMEDIASUBTYPE_RGB8;
								}
								else
								{
									mt.subtype = GUID_NULL;
								}
							}
							mt.bFixedSizeSamples	= false;
							mt.bTemporalCompression	= false;
							mt.lSampleSize			= 0;
							mt.formattype			= WMFORMAT_VideoInfo;
							mt.pUnk					= NULL;
							mt.cbFormat				= sizeof(WMVIDEOINFOHEADER);
							mt.pbFormat				= (BYTE*)&videoInfo;

							if(FAILED(m_pVideoProps->SetMediaType(&mt)))
							{
								SetErrorMessage(L"Unable to Set Media Type");
								return E_FAIL;
							}

							if(FAILED(m_pVideoWMWriter->SetInputProps(m_dwVideoInput,m_pVideoProps)))
							//HRESULT res = m_pWMWriter->SetInputProps(m_dwVideoInput,m_pVideoProps);
							{																
								//GetErrorMessage(_com_error(res));								
								SetErrorMessage(L"Unable to Set Input Properties for Media Writer");
								return E_FAIL;
							}


							//
							// Setup the local copy of the uncompressed media type
							//
							WORD wNumChannels = 1;
							WORD wBitsPerSample = 8;
							DWORD dwSamplesPerSecond;
							WM_MEDIA_TYPE mtUncompressedAudio;
							WAVEFORMATEX wfxUncompressedAudio;
							ZeroMemory( &mtUncompressedAudio, sizeof( mtUncompressedAudio ) );

							mtUncompressedAudio.majortype = WMMEDIATYPE_Audio;
							mtUncompressedAudio.subtype = WMMEDIASUBTYPE_WMAudioV9;
							mtUncompressedAudio.bFixedSizeSamples = TRUE;
							mtUncompressedAudio.bTemporalCompression = FALSE;
							mtUncompressedAudio.lSampleSize = wNumChannels * wBitsPerSample / 8;
							mtUncompressedAudio.formattype = WMFORMAT_WaveFormatEx;
							mtUncompressedAudio.pUnk = NULL;
							mtUncompressedAudio.cbFormat = sizeof( WAVEFORMATEX );
							mtUncompressedAudio.pbFormat = (BYTE*) &wfxUncompressedAudio;

							//
							// Configure the WAVEFORMATEX structure for the uncompressed audio
							//
							ZeroMemory( &wfxUncompressedAudio, sizeof( wfxUncompressedAudio ) );

							wfxUncompressedAudio.wFormatTag = 1;
							wfxUncompressedAudio.nChannels = wNumChannels;
							wfxUncompressedAudio.nSamplesPerSec = 44100;
							wfxUncompressedAudio.nAvgBytesPerSec = dwSamplesPerSecond * ( wNumChannels * wBitsPerSample / 8 );
							wfxUncompressedAudio.nBlockAlign = wNumChannels * wBitsPerSample / 8;
							wfxUncompressedAudio.wBitsPerSample = wBitsPerSample;
							wfxUncompressedAudio.cbSize = sizeof( WAVEFORMATEX );

							if(FAILED(m_pAudioProps->SetMediaType(&mtUncompressedAudio)))
							{
								SetErrorMessage(L"Unable to Set Media Type");
								return E_FAIL;
							}


							//if(FAILED(m_pAudioWMWriter->SetInputProps(m_dwAudioInput,m_pAudioProps)))
							HRESULT res2 = m_pAudioWMWriter->SetInputProps(m_dwAudioInput,m_pAudioProps);
							{																
								GetErrorMessage(_com_error(res2));								
								SetErrorMessage(L"Unable to Set Input Properties for Media Writer");
								return E_FAIL;
							}

							if(FAILED(m_pVideoWMWriter->BeginWriting()))
							//HRESULT res2 = m_pWMWriter->BeginWriting();
							{
								//GetErrorMessage(_com_error(res2));								
								SetErrorMessage(L"Unable to Initialize Writing");
								return E_FAIL;
							}

							//if(FAILED(m_pAudioWMWriter->BeginWriting()))
							HRESULT res3 = m_pAudioWMWriter->BeginWriting();
							{
								GetErrorMessage(_com_error(res3));								
								SetErrorMessage(L"Unable to Initialize Writing");
								return E_FAIL;
							}


							return S_OK;
						}


						HRESULT	CwmvFileWithAudio::AppendFrameFirstTime(HBITMAP	hBitmap)
						{
							BITMAP Bitmap;

							GetObject(hBitmap, sizeof(BITMAP), &Bitmap);

							if(SUCCEEDED(InitMovieCreation( Bitmap.bmWidth, 
								Bitmap.bmHeight, 
								Bitmap.bmBitsPixel)))
							{
								m_nAppendFuncSelector=2;		//Point to UsualAppend Function

								return AppendFrameUsual(hBitmap);
							}

							//Control Comes here Only if there is Any Error

							ReleaseMemory();

							return E_FAIL;
						}

						HRESULT CwmvFileWithAudio::AppendFrameUsual(HBITMAP hBitmap)
						{
							HRESULT			hr=E_FAIL;
							INSSBuffer		*pSample=NULL;
							BYTE			*pbBuffer=NULL;
							DWORD			cbBuffer=0;

							BITMAPINFO	bmpInfo;

							bmpInfo.bmiHeader.biBitCount=0;
							bmpInfo.bmiHeader.biSize=sizeof(BITMAPINFOHEADER);

							GetDIBits(m_hwmvDC,hBitmap,0,0,NULL,&bmpInfo,DIB_RGB_COLORS);

							bmpInfo.bmiHeader.biCompression=BI_RGB;

							if(FAILED(m_pVideoWMWriter->AllocateSample(bmpInfo.bmiHeader.biSizeImage,&pSample)))
							{
								SetErrorMessage(L"Unable to Allocate Memory");
								goto TerminateAppendWMVFrameUsual;
							}

							if(FAILED(pSample->GetBufferAndLength(&pbBuffer,&cbBuffer)))
							{
								SetErrorMessage(L"Unable to Lock Buffer");
								goto TerminateAppendWMVFrameUsual;
							}

							GetDIBits(m_hwmvDC,hBitmap,0,bmpInfo.bmiHeader.biHeight,(void*)pbBuffer,&bmpInfo,DIB_RGB_COLORS);

							hr=m_pVideoWMWriter->WriteSample(m_dwVideoInput,10000 * m_msVideoTime,0,pSample);							

							m_msVideoTime=(++m_dwCurrentVideoSample*1000)/m_dwFrameRate;

							if(pSample)	{	pSample->Release();	pSample=NULL;	}

							if(FAILED(hr))
							{
								SetErrorMessage(L"Unable to Write Frame");
								goto TerminateAppendWMVFrameUsual;
							}

							return S_OK;

TerminateAppendWMVFrameUsual:

							if(pSample)	{	pSample->Release();	pSample=NULL;	}

							ReleaseMemory();

							return E_FAIL;
						}

						HRESULT CwmvFileWithAudio::AppendDummy(HBITMAP)
						{
							return E_FAIL;
						}

						HRESULT CwmvFileWithAudio::AppendNewFrame(HBITMAP hBitmap)
						{
							return (this->*pAppendFrame[m_nAppendFuncSelector])(hBitmap);
						}

						HRESULT CwmvFileWithAudio::AppendNewFrame(int nWidth, int nHeight, LPVOID pBits,int nBitsPerPixel)
						{
							return (this->*pAppendFrameBits[m_nAppendFuncSelector])(nWidth,nHeight,pBits,nBitsPerPixel);
						}

						HRESULT	CwmvFileWithAudio::AppendFrameFirstTime(int nWidth, int nHeight, LPVOID pBits,int nBitsPerPixel)
						{
							if(SUCCEEDED(InitMovieCreation(nWidth, nHeight, nBitsPerPixel)))
							{
								m_nAppendFuncSelector=2;		//Point to UsualAppend Function

								return AppendFrameUsual(nWidth,nHeight,pBits,nBitsPerPixel);
							}

							// Control Comes here Only if there is Any Error
							ReleaseMemory();

							return E_FAIL;
						}


						HRESULT	CwmvFileWithAudio::AppendFrameUsual(int nWidth, int nHeight, LPVOID pBits,int nBitsPerPixel)
						{
							HRESULT			hr=E_FAIL;
							INSSBuffer		*pSample=NULL;
							BYTE			*pbBuffer=NULL;
							DWORD			cbBuffer=0;

							BITMAPINFO	bmpInfo;

							ZeroMemory(&bmpInfo,sizeof(BITMAPINFO));
							bmpInfo.bmiHeader.biSize=sizeof(BITMAPINFOHEADER);
							bmpInfo.bmiHeader.biBitCount=nBitsPerPixel;

							bmpInfo.bmiHeader.biWidth=nWidth;
							bmpInfo.bmiHeader.biHeight=nHeight;	

							bmpInfo.bmiHeader.biCompression=BI_RGB;

							bmpInfo.bmiHeader.biPlanes=1;
							bmpInfo.bmiHeader.biSizeImage=nWidth*nHeight*nBitsPerPixel/8;

							if(FAILED(m_pVideoWMWriter->AllocateSample(bmpInfo.bmiHeader.biSizeImage,&pSample)))
							{
								SetErrorMessage(L"Unable to Allocate Memory");
								goto TerminateAppendWMVFrameUsualBits;
							}

							if(FAILED(pSample->GetBufferAndLength(&pbBuffer,&cbBuffer)))
							{
								SetErrorMessage(L"Unable to Lock Buffer");
								goto TerminateAppendWMVFrameUsualBits;
							}

							memcpy(pbBuffer,pBits,bmpInfo.bmiHeader.biSizeImage);

							//hr=m_pWMWriter->WriteSample(m_dwVideoInput,10000 * m_msVideoTime,0,pSample);
							hr=m_pVideoWMWriter->WriteSample(m_dwVideoInput,100 * m_msVideoTime,0,pSample);

							m_msVideoTime=(++m_dwCurrentVideoSample*1000)/m_dwFrameRate;

							if(pSample)	{	pSample->Release();	pSample=NULL;	}

							if(FAILED(hr))
							{
								SetErrorMessage(L"Unable to Write Frame");
								goto TerminateAppendWMVFrameUsualBits;
							}

							return S_OK;

TerminateAppendWMVFrameUsualBits:

							if(pSample)	{	pSample->Release();	pSample=NULL;	}

							ReleaseMemory();

							return E_FAIL;
						}

						HRESULT	CwmvFileWithAudio::AppendDummy(int nWidth, int nHeight, LPVOID pBits,int nBitsPerPixel)
						{
							return E_FAIL;
						}
					}
				}
			}
		}
	}
}

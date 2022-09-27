#include "utils/Pattern.h"
#include <string>

Pattern::Pattern(unsigned char* pattern, unsigned char* mask, int count)
	: count(count)
{
	if (count <= 0)
		return;

	if (pattern) {
		this->pattern = new unsigned char[count];
		memcpy(this->pattern, pattern, count);
	}

	if (mask) {
		this->mask = new unsigned char[count];
		memcpy(this->mask, mask, count);
	}
}

Pattern::~Pattern()
{
	if (this->pattern)
	{
		delete[] this->pattern;
		this->pattern = NULL;
	}

	if (this->mask)
	{
		delete[] this->mask;
		this->mask = NULL;
	}
}

bool Pattern::Match(void* buffer) const
{
	// 
	if (this->count <= 0 || !this->pattern)
		return false;

	unsigned char* compareBuffer = (unsigned char*)buffer;
	for (int i = 0; i < this->count; ++i)
	{
		unsigned char pValue = this->pattern[i];
		unsigned char cValue = compareBuffer[i];
		unsigned char mask = this->mask ? this->mask[i] : 0xFF;

		if ((pValue & mask) != (cValue & mask))
			return false;
	}

	return true;
}

int Pattern::GetCount() const
{
	return this->count;
}

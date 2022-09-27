#pragma once

class Pattern
{
	unsigned char* pattern;
	unsigned char* mask;
	int count;

public:
	Pattern(unsigned char* pattern, unsigned char* mask, int count);
	~Pattern();

	bool Match(void* buffer) const;

	int GetCount() const;
};

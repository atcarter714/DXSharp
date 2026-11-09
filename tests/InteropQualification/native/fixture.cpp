#include <atomic>
#include <cstdint>
#include <cstring>
#ifdef _WIN32
#include <directml.h>
#define ABI_CALL __stdcall
#define EXPORT extern "C" __declspec(dllexport)
#else
#define ABI_CALL
#define EXPORT extern "C" __attribute__((visibility("default")))
#endif

struct Guid { std::uint32_t a; std::uint16_t b, c; std::uint8_t d[8]; };
struct Pair { std::uint64_t first, second; };
static const Guid unknown_id = { 0, 0, 0, {0xc0,0,0,0,0,0,0,0x46} };
static const Guid fixture_id = { 0x96446a17, 0x3c17, 0x451c, {0xbc,0x9d,0x5b,0xb5,0x59,0x18,0x0b,0x54} };
static const Guid derived_id = { 0x1659e483, 0x98ab, 0x4e6c, {0x89,0xab,0x0f,0x42,0xf2,0x70,0x2c,0x71} };
static std::atomic<int> alive{0}, destroyed{0};

struct Unknown {
    virtual std::int32_t ABI_CALL QueryInterface(const Guid*, void**) = 0;
    virtual std::uint32_t ABI_CALL AddRef() = 0;
    virtual std::uint32_t ABI_CALL Release() = 0;
};
struct Fixture : Unknown {
    virtual std::int32_t ABI_CALL Echo(std::int32_t) = 0;
    virtual void ABI_CALL SetValue(std::int32_t) = 0;
    virtual std::int32_t ABI_CALL GetValue() = 0;
    virtual std::int32_t ABI_CALL Fill(std::uint8_t*, std::uint32_t) = 0;
};
struct Derived : Fixture { virtual Pair ABI_CALL ReadPair() = 0; };
struct Callback : Unknown { virtual std::int32_t ABI_CALL Invoke(std::int32_t) = 0; };

class Object final : public Derived {
    std::atomic<std::uint32_t> refs{1};
    std::int32_t value{0};
public:
    Object() { ++alive; }
    ~Object() { --alive; ++destroyed; }
    std::int32_t ABI_CALL QueryInterface(const Guid* iid, void** output) override {
        *output = nullptr;
        if (std::memcmp(iid, &unknown_id, sizeof(Guid)) && std::memcmp(iid, &fixture_id, sizeof(Guid)) && std::memcmp(iid, &derived_id, sizeof(Guid))) return static_cast<std::int32_t>(0x80004002u);
        *output = static_cast<Derived*>(this);
        AddRef();
        return 0;
    }
    std::uint32_t ABI_CALL AddRef() override { return ++refs; }
    std::uint32_t ABI_CALL Release() override { auto count = --refs; if (!count) delete this; return count; }
    std::int32_t ABI_CALL Echo(std::int32_t input) override { return input ^ 0x13579; }
    void ABI_CALL SetValue(std::int32_t input) override { value = input; }
    std::int32_t ABI_CALL GetValue() override { return value; }
    std::int32_t ABI_CALL Fill(std::uint8_t* output, std::uint32_t count) override {
        if (!output && count) return static_cast<std::int32_t>(0x80004003u);
        for (std::uint32_t i = 0; i < count; ++i) output[i] = static_cast<std::uint8_t>(i ^ 0x5a);
        return 0;
    }
    Pair ABI_CALL ReadPair() override { return {0x1122334455667788ULL, 0x8877665544332211ULL}; }
};

static Callback* retained = nullptr;
EXPORT void* fixture_create() { return static_cast<Derived*>(new Object()); }
EXPORT int fixture_alive() { return alive.load(); }
EXPORT int fixture_destroyed() { return destroyed.load(); }
#ifndef _WIN32
struct DML_BUFFER_BINDING { void* Buffer; std::uint64_t Offset, SizeInBytes; };
struct DML_BUFFER_TENSOR_DESC {
    std::uint32_t DataType, Flags, DimensionCount; const std::uint32_t *Sizes, *Strides;
    std::uint64_t TotalTensorSizeInBytes; std::uint32_t GuaranteedBaseOffsetAlignment;
};
#endif
EXPORT int fixture_size(int index) {
    switch(index) {
    case 0: return sizeof(Guid);
    case 1: return sizeof(Pair);
    case 2: return sizeof(void*);
    case 3: return sizeof(DML_BUFFER_BINDING);
    case 4: return sizeof(DML_BUFFER_TENSOR_DESC);
    default: return -1;
    }
}
EXPORT void fixture_retain_callback(Callback* callback) { if (retained) retained->Release(); retained = callback; if (retained) retained->AddRef(); }
EXPORT int fixture_invoke_callback(int input) { return retained ? retained->Invoke(input) : -1; }

struct Blob : Unknown {
    virtual void* ABI_CALL GetBufferPointer() = 0;
    virtual std::size_t ABI_CALL GetBufferSize() = 0;
};
struct Include : Unknown {
    virtual std::int32_t ABI_CALL LoadSource(const char16_t*, Blob**) = 0;
};
EXPORT int fixture_include(void* pointer) {
    auto handler = static_cast<Include*>(pointer);
    Blob* blob = nullptr;
    auto hr = handler->LoadSource(u"fixture.hlsli", &blob);
    if (hr < 0 || !blob) return -1;
    const char expected[] = "generated include";
    bool valid = blob->GetBufferSize() == sizeof(expected) - 1
        && std::memcmp(blob->GetBufferPointer(), expected, sizeof(expected) - 1) == 0;
    blob->Release();
    return valid ? 1 : 0;
}
